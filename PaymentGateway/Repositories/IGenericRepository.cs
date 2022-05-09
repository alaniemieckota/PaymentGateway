namespace PaymentGateway.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> Get(string id);
    
    Task Add(T entity);
    
    void Update(T entity);
}
