using Contracts.BoardDTOs;
using Contracts.TransactionDTOs;
using dataaccess;
using dataaccess.Entities;
using service.Services.Interfaces;

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
    
    public PurchaseService(ITransactionService transactionService, IBoardService boardService, AppDbContext dbContext, IUserService userService)
    {
        _transactionService = transactionService;
        _boardService = boardService;
        _dbContext = dbContext;
        _userService = userService;
    } 
    
    public async Task<bool> ProcessPurchaseAsync(CreateBoardDto boardDto, CreateTransactionDto transactionDto)
    {
        var user = _userService.GetByIdAsync(transactionDto.UserID);
        var userBalance = user.Result.Balance;
        PriceConfig.Values.TryGetValue(boardDto.BoardSize, out var price);

        if (price <= userBalance)
        {
            transactionDto.Amount = -price * boardDto.Week;
        
            await using var tx = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _transactionService.CreateAsync(transactionDto);
                await _boardService.CreateAsync(boardDto);
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
               // throw new Exception ("Det var ikke muligt at gennemføre købet.");
                // return Exception;
            }
        }
        return false;
        
        

        
    }

}