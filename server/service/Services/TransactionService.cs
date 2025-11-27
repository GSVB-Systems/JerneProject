using dataaccess.Entities;
using Service.Repositories;

namespace service.Services;

public class TransactionService : Service<Transaction>
{
    public TransactionService(ITransactionRepository transactionRepositoryRepository) : base(transactionRepositoryRepository)
    {
    }
}