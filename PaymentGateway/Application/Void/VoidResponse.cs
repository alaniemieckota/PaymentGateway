namespace PaymentGateway.Application.Void;

using PaymentGateway.Application.SharedModels;

public record VoidResponse(AmountModel amount, string Status, string Description);
