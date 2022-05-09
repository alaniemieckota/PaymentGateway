namespace PaymentGateway.Application.Refund;

using FluentValidation;

public class RefundValidator : AbstractValidator<RefundRequest>
{
    public RefundValidator()
    {
        // TODO: add more validations
        RuleFor(r => r.Amount).GreaterThan(0);
    }
}
