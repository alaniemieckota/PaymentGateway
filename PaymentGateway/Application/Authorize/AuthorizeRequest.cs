using MediatR;

namespace PaymentGateway.Application.Authorize
{
    public record AuthorizeRequest(
        string CardHolderName,
        string CardNumber,
        int ExpiryYear,
        int ExpiryMonth,
        string CVV,
        decimal Amount,
        string Currency
        ) : IRequest<AuthorizeResponse>;
}
