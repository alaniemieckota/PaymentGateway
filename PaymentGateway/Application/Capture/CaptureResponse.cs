namespace PaymentGateway.Application.Capture;

using PaymentGateway.Application.SharedModels;

public record CaptureResponse(AmountModel Amount, string Status, string Description);
