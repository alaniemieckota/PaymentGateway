namespace PaymentGateway.Services.Models;

public record PaymentProcessorAuthorizationResult(bool WasAuthorized, string RejectionReason);

