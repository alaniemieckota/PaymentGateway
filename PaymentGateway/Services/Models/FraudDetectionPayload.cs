namespace PaymentGateway.Services.Models;

public record FraudDetectionPayload(
    string CardHolderName,
    string CardNumber,
    int ExpiryYear,
    int ExpiryMonth,
    string CVV,
    decimal Amount,
    string Currency
    );