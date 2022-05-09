namespace PaymentGateway.Application.Capture;

using FluentValidation;

public class CaptureValidator : AbstractValidator<CaptureRequest>
{
    public CaptureValidator()
    {
        RuleFor(c => c.Amount).GreaterThan(0);
    }
}
