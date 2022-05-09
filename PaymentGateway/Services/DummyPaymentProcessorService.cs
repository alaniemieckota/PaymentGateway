namespace PaymentGateway.Services;

using PaymentGateway.Services.Models;

public class DummyPaymentProcessorService : IPaymentProcessorService
{
    public async Task<PaymentProcessorAuthorizationResult> RequestAuthorization(
        string cardHolderName,
        string cardNumber,
        int expiryYear,
        int expiryMonth,
        string cvv,
        decimal amount,
        string currency)
    {
        var random = new Random(Guid.NewGuid().GetHashCode());
        await Task.Delay(random.Next(1000)); // simulate response delay
        
        if (cardNumber == "4000000000000119")
        {
            return new PaymentProcessorAuthorizationResult(false, "Card closed");
        }

        return new PaymentProcessorAuthorizationResult(true, string.Empty);
    }
}
