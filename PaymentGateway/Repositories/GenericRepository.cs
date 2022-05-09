namespace PaymentGateway.Repositories;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext context;

    protected GenericRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<T?> Get(string id)
    {
        return await this.context.Set<T>().FindAsync(id);
    }

    public async Task Add(T entity)
    {
        await this.context.Set<T>().AddAsync(entity);
    }
    
    public void Update(T entity)
    {
        this.context.Set<T>().Update(entity);
    }
}