namespace PaymentGateway.Application.Refund;

using MediatR;

public record RefundRequest(string AuthorizationId, decimal Amount) : IRequest<RefundResponse>;
