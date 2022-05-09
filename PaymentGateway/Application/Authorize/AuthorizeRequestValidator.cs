namespace PaymentGateway.Application.Authorize;

using FluentValidation;

public class AuthorizeRequestValidator : AbstractValidator<AuthorizeRequest>
{
    public AuthorizeRequestValidator()
    {
        var supportedCurrencies = new[] { "EUR", "USD" }; // Should be taken from business rules/settings
        this.ClassLevelCascadeMode = CascadeMode.Stop; // validation does not continue, important for expiry validation

        //RuleFor(v => v.CVV).MinimumLength(3).MaximumLength(4);
        //RuleFor(v => v.Amount).GreaterThan(0);
        //RuleFor(v => v.CardNumber)
        //    .MinimumLength(12)
        //    .MaximumLength(19)
        //    .Must(cc => new LuhnValidator().IsValid(cc))
        //    .WithMessage("Card Number is not valid.");
        //RuleFor(v => v.ExpiryYear).GreaterThanOrEqualTo(2022);
        //RuleFor(v => v.ExpiryMonth).InclusiveBetween(1,12);
        //RuleFor(v => v).Must(r => ValidateExpiry(r)).WithMessage("Card expired.");
        //RuleFor(v => v.CardHolderName).MinimumLength(3).MaximumLength(128);
        //RuleFor(v => v.Currency)
        //    .NotEmpty()
        //    .Must(currency => supportedCurrencies.Contains(currency))
        //    .WithMessage("Not supported currency.");
    }

    private bool ValidateExpiry(AuthorizeRequest request)
    {
        var ccExpiryDate = 
            new DateTime(
                request.ExpiryYear,
                request.ExpiryMonth,
                DateTime.DaysInMonth(request.ExpiryYear, request.ExpiryMonth));
        
        return ccExpiryDate > DateTime.UtcNow;
    }
}
