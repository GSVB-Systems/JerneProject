using dataaccess;
using dataaccess.Entities;
using service.Repositories.Interfaces;

namespace service.Repositories;


public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    
    public TransactionRepository(AppDbContext context) : base(context)
    {
    }
    
}