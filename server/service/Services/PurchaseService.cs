using Contracts.BoardDTOs;
using Contracts.TransactionDTOs;
using dataaccess;
using service.Services.Interfaces;

namespace service.Services;

public class PurchaseService: IPurchaseService
{
    
    private readonly ITransactionService _transactionService;
    private readonly IBoardService _boardService;
    private readonly AppDbContext _dbContext;
    
    public PurchaseService(ITransactionService transactionService, IBoardService boardService, AppDbContext dbContext)
    {
        _transactionService = transactionService;
        _boardService = boardService;
        _dbContext = dbContext;
    } 
    
    public async Task<bool> ProcessPurchaseAsync(CreateBoardDto boardDto, CreateTransactionDto transactionDto)
    {
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
            throw;
        }
    }

}