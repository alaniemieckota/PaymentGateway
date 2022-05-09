namespace PaymentGateway.Repositories;

public interface IUnitOfWork
{
    IAuthorizationRepository AuthorizationRepository { get; }

    ITransactionRepository TransactionRepository { get; }

    Task<int> Complete();

    void BeginTransaction();
}
