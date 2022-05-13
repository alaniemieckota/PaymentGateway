namespace PaymentGatewayTests.Application.Authorize;

using NUnit.Framework;
using PaymentGateway.Application.Authorize;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Authentication;
using Moq;

public class AuthorizeRequestValidatorTests
{
    private AuthorizeRequestValidator target = null!;

    private const int NowYear = 2022;

    private const int NowMonth = 5;

    [SetUp]
    public void SetUp()
    {
        var systemClockMoq = new Mock<ISystemClock>();
        systemClockMoq.SetupGet(m => m.UtcNow).Returns(new System.DateTimeOffset(new System.DateTime(NowYear, NowMonth, 1)));
        this.target = new AuthorizeRequestValidator(systemClockMoq.Object);
    }

    [TestCase("4111111111111111")]
    [TestCase("4000000000000119")]
    [TestCase("4000000000000259")]
    [TestCase("4000000000003238")]
    [TestCase("5425233430109903")]
    public void Validate_WhenValidData_ShouldPass(string validCreditCarNumber)
    {
        // Arrange
        var authorizationRequest = CreateAuthorizationRequest(cardNumber: validCreditCarNumber);

        // Act
        var actual = this.target.TestValidate(authorizationRequest);
        
        // Assert
        Assert.True(actual.IsValid);
    }

    [TestCase("411112211111")]
    [TestCase("4000 00119")]
    [TestCase("4123000000259")]
    [TestCase("4000sdf00 3238")]
    [TestCase("This is not a valid one")]
    [TestCase("4263982640269298")]
    public void Validate_WhenIncorrectCardNumber_ShouldNotBeValidAndHaveErrorOnCardNumber(string incorrectCreditCardNumber)
    {
        // Arrange
        var authorizationRequest = CreateAuthorizationRequest(cardNumber: incorrectCreditCardNumber);

        // Act
        var actual = this.target.TestValidate(authorizationRequest);

        // Assert
        Assert.False(actual.IsValid);
        actual.ShouldHaveValidationErrorFor(x => x.CardNumber);
    }

    [TestCase(0)]
    [TestCase(-2)]
    public void Validate_WhenIncorrectAmount_ShouldNotBeValidAndHaveErrorOnAmount(decimal incorrectAmount)
    {
        // Arrange
        var authorizationRequest = CreateAuthorizationRequest(amount: incorrectAmount);

        // Act
        var actual = this.target.TestValidate(authorizationRequest);

        // Assert
        Assert.False(actual.IsValid);
        actual.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [TestCase("12")]
    [TestCase("123545")]
    public void Validate_WhenInvalidYear_ShouldNotBeValidAndHaveErrorOnCvv(string invalidCvv)
    {
        // Arrange
        var authorizationRequest = CreateAuthorizationRequest(cvv: invalidCvv);

        // Act
        var actual = this.target.TestValidate(authorizationRequest);

        // Assert
        Assert.False(actual.IsValid);
        actual.ShouldHaveValidationErrorFor(x => x.CVV);
    }

    [TestCase(2020)]
    [TestCase(2000)]
    public void Validate_WhenInvalidYear_ShouldNotBeValidAndHaveErrorOnExpiryYear(int expiryYearOnCard)
    {
        // Arrange
        var authorizationRequest = CreateAuthorizationRequest(expiryYear: expiryYearOnCard);

        // Act
        var actual = this.target.TestValidate(authorizationRequest);

        // Assert
        Assert.False(actual.IsValid);
        actual.ShouldHaveValidationErrorFor(x => x.ExpiryYear);
    }

    [TestCase(0)]
    [TestCase(24)]
    public void Validate_WhenInvalidMonth_ShouldNotBeValidAndHaveErrorOnExpiryMonth(int expiryMonthOnCard)
    {
        // Arrange
        var authorizationRequest = CreateAuthorizationRequest(expiryMonth: expiryMonthOnCard);

        // Act
        var actual = this.target.TestValidate(authorizationRequest);

        // Assert
        Assert.False(actual.IsValid);
        actual.ShouldHaveValidationErrorFor(x => x.ExpiryMonth);
    }

    [TestCase(NowYear, 1)]
    [TestCase(NowYear, 1)]
    [TestCase(1990, NowMonth)]
    [TestCase(NowYear, 4)]
    public void Validate_WhenInvalidMonth_ShouldNotBeValidAndHaveErrorOnExpiryMonth(int expiryYear, int expiryMonthOnCard)
    {
        // Arrange
        var authorizationRequest = CreateAuthorizationRequest(expiryYear:expiryYear, expiryMonth: expiryMonthOnCard);

        // Act
        var actual = this.target.TestValidate(authorizationRequest);

        // Assert
        Assert.False(actual.IsValid);
        actual.ShouldHaveAnyValidationError();
    }

    [TestCase("J")]
    [TestCase("This is very log text which is not suppossed to be so long but shoud be just a card holder name.")]
    public void Validate_WhenInvalidCardHolderName_ShouldNotBeValidAndHaveErrorOnCardHolderName(string invalidCardHolderName)
    {
        // Arrange
        var authorizationRequest = CreateAuthorizationRequest(cardHolderName: invalidCardHolderName);

        // Act
        var actual = this.target.TestValidate(authorizationRequest);

        // Assert
        Assert.False(actual.IsValid);
        actual.ShouldHaveValidationErrorFor(x => x.CardHolderName);
    }

    [TestCase("ETH")]
    [TestCase("W")]
    public void Validate_WhenInvalidCurrency_ShouldNotBeValidAndHaveErrorOnCurrency(string invalidCurrency)
    {
        // Arrange
        var authorizationRequest = CreateAuthorizationRequest(currency: invalidCurrency);

        // Act
        var actual = this.target.TestValidate(authorizationRequest);

        // Assert
        Assert.False(actual.IsValid);
        actual.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    private AuthorizeRequest CreateAuthorizationRequest
        (
            decimal amount = 100.99m,
            string currency = "USD",
            string cardHolderName = "John Doe",
            string cardNumber = "374245455400126",
            int expiryYear = 2050,
            int expiryMonth = 5,
            string cvv = "123"
            
        )
    {
        return new AuthorizeRequest
            (
                CardHolderName: cardHolderName,
                CardNumber: cardNumber,
                ExpiryYear: expiryYear,
                ExpiryMonth: expiryMonth,
                Amount: amount,
                CVV: cvv,
                Currency: currency
            );   
    }
}
