using dataaccess;
using dataaccess.Entities;

namespace Service.Repositories;


public class TransactionRepositoryRepository : Repository<Transaction>, ITransactionRepository
{
    
    public TransactionRepositoryRepository(AppDbContext context) : base(context)
    {
    }
    
}