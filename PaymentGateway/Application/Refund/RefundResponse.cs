namespace PaymentGateway.Application.Refund;

using PaymentGateway.Application.SharedModels;

public record RefundResponse(AmountModel Amount, string Status, string Description);