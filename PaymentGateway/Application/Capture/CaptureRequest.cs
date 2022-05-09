namespace PaymentGateway.Application.Capture;

using MediatR;

public record CaptureRequest(string AuthorizationId, decimal Amount) : IRequest<CaptureResponse>;
