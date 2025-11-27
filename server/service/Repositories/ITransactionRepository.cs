using dataaccess;
using dataaccess.Entities;

namespace Service.Repositories;


public class TransactionRepository : Repository<Transaction>, ITransaction
{
    
    public TransactionRepository(AppDbContext context) : base(context)
    {
    }
    
}