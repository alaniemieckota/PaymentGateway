namespace PaymentGateway.Infrastructure;

public interface ICurrentCallerContext
{
    string GetCallerId();
}
