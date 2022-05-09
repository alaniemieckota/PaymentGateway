namespace PaymentGateway.Services;

using PaymentGateway.Services.Models;

public interface IPaymentProcessorService
{
    Task<PaymentProcessorAuthorizationResult> RequestAuthorization(
        string cardHolderName,
        string cardNumber,
        int expiryYear,
        int expiryMonth,
        string cvv,
        decimal amount,
        string currency);
}
