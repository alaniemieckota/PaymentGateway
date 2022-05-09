namespace PaymentGateway.Services;

public interface IIdempotencyService 
{
    Task CheckUniqness(string idempotencyKey);
}