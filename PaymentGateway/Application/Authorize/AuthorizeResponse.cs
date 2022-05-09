namespace PaymentGateway.Application.Authorize;

using PaymentGateway.Application.SharedModels;


public record AuthorizeResponse(string AuthorizationId, AmountModel amount, string Status, string Description);