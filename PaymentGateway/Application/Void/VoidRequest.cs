namespace PaymentGateway.Application.Void;

using MediatR;

public record VoidRequest(string AuthorizationId) : IRequest<VoidResponse>;
