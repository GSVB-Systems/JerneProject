using Contracts.BoardDTOs;
using Contracts.TransactionDTOs;
using dataaccess;
using service.Rules.RuleInterfaces;
using service.Services.Interfaces;
using service.Validators;

namespace service.Services;

public class PurchaseService: IPurchaseService
{
    public static class PriceConfig
    {
        public static readonly IReadOnlyDictionary<int, int> Values =
            new Dictionary<int, int>
            {
                { 5, 20 },
                { 6, 40 },
                { 7, 80 },
                { 8, 160 }
            };
    }

    private readonly ITransactionService _transactionService;
    private readonly IBoardService _boardService;
    private readonly AppDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IPurchaseRules _purchaseRules;

    public PurchaseService(
        ITransactionService transactionService,
        IBoardService boardService,
        AppDbContext dbContext,
        IUserService userService,
        IPurchaseRules purchaseRules)
    {
        _transactionService = transactionService;
        _boardService = boardService;
        _dbContext = dbContext;
        _userService = userService;
        _purchaseRules = purchaseRules ?? throw new ArgumentNullException(nameof(purchaseRules));
    }

    public async Task<bool> ProcessPurchaseAsync(CreateBoardDto board, CreateTransactionDto transaction)
    {
        
        var momentUtc = ExtractTransactionMomentUtc(transaction);
        PurchaseWindowValidator.EnsurePurchaseAllowed(momentUtc);

        await _purchaseRules.ValidateProcessPurchaseAsync(board, transaction);

        var user = await _userService.GetByIdAsync(transaction.UserID);
        if (user == null) return false;

        var userBalance = user.Balance;

        if (!PriceConfig.Values.TryGetValue(board.BoardSize, out var price))
            return false;

        if (price <= userBalance)
        {
            transaction.Amount = -price * board.Week;

            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _transactionService.CreateAsync(transaction);
                await _boardService.CreateAsync(board);
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
            }
        }

        return false;
    }

    
    private static DateTime? ExtractTransactionMomentUtc(CreateTransactionDto dto)
    {
        if (dto == null) return null;

        var prop = dto.GetType().GetProperties()
            .FirstOrDefault(p => string.Equals(p.Name, "TransactionDate", StringComparison.OrdinalIgnoreCase));
        if (prop == null) return null;

        var val = prop.GetValue(dto);
        if (val == null) return null;

        if (val is DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Unspecified)
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return dt.ToUniversalTime();
        }

        if (val is DateTimeOffset dtoff)
            return dtoff.UtcDateTime;

        if (val is string s)
        {
            if (DateTimeOffset.TryParse(s, out var parsed))
                return parsed.UtcDateTime;
        }

        return null;
    }

}