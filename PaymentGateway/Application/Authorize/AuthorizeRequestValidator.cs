namespace PaymentGateway.Application.Authorize;

using FluentValidation;
using Microsoft.AspNetCore.Authentication;

public class AuthorizeRequestValidator : AbstractValidator<AuthorizeRequest>
{
    public AuthorizeRequestValidator(ISystemClock systemClock)
    {
        var supportedCurrencies = new[] { "EUR", "USD" }; // Should be taken from business rules/settings
        this.ClassLevelCascadeMode = CascadeMode.Stop; // validation does not continue, important for expiry validation

        RuleFor(v => v.CVV).MinimumLength(3).MaximumLength(4);
        RuleFor(v => v.Amount).GreaterThan(0);
        RuleFor(v => v.CardNumber)
            .MinimumLength(12)
            .MaximumLength(19)
            .Must(cc => ValidateCreditCardNumber(cc))
            .WithMessage("Card Number is not valid.");
        RuleFor(v => v.ExpiryYear).GreaterThanOrEqualTo(systemClock.UtcNow.Year);
        RuleFor(v => v.ExpiryMonth).InclusiveBetween(1,12);
        RuleFor(v => v).Must(r => ValidateExpiry(r)).WithMessage("Card expired.");
        RuleFor(v => v.CardHolderName).MinimumLength(2).MaximumLength(40);
        RuleFor(v => v.Currency)
            .NotEmpty()
            .Must(currency => supportedCurrencies.Contains(currency))
            .WithMessage("Not supported currency.");
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

    private bool ValidateCreditCardNumber(string ccNumber)
    {
        var doAlternate = false;
        var sum = 0;

        if (ccNumber.Length < 13
            || ccNumber.Length > 19)
        {
            return false;
        }

        for (int i = ccNumber.Length - 1; i > -1; i--)
        {
            var a = ccNumber[i] - '0';
            Console.WriteLine(a);
            if (!int.TryParse(ccNumber[i].ToString(), out var mod))
            {
                return false; // just in case the cc number has something else than a digit
            }

            if (doAlternate)
            {
                mod *= 2;
                if (mod > 9)
                {
                    mod = mod % 10 + 1;
                }
            }

            doAlternate = !doAlternate;
            sum += mod;
        }

        return sum % 10 == 0;
    }
}
