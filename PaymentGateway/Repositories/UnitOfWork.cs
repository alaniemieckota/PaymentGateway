namespace PaymentGateway.Repositories;

using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    public IAuthorizationRepository AuthorizationRepository { get; }
    
    public ITransactionRepository TransactionRepository { get; }

    private readonly ApplicationDbContext context;

    private IDbContextTransaction transaction = null!;


    public UnitOfWork(ApplicationDbContext applicationDbContext,
        IAuthorizationRepository authorizationsRepository,
        ITransactionRepository transactionRepository)
    {
        this.context = applicationDbContext;
        this.AuthorizationRepository = authorizationsRepository;
        this.TransactionRepository = transactionRepository;
    }

    public void BeginTransaction()
    {
        if(this.transaction != null)
        {
            throw new InvalidOperationException("Transaction already exists");
        }

        try
        {
            this.transaction = this.context.Database.BeginTransaction();
        }
        catch (InvalidOperationException) { } // InMemmoryDb is not supporting transactions
    }

    public async Task<int> Complete()
    {      
        var numberOfChanges = await this.context.SaveChangesAsync();

        if(this.transaction != null)
        {
            await this.transaction.CommitAsync();
            this.transaction = null!;
        }

        return numberOfChanges;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.transaction?.Dispose();
            this.context.Dispose();
        }
    }
}

