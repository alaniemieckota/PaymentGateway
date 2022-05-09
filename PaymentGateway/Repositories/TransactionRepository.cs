namespace PaymentGateway.Repositories;

using PaymentGateway.Repositories.Dtos;

public class TransactionRepository : GenericRepository<TransactionDto>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext context) : base(context)
    {
    }
}