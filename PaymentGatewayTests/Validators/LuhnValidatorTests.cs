using NUnit.Framework;
using PaymentGateway.Validators;

namespace PaymentGatewayTests.Validators
{
    public class LuhnValidatorTests
    {
        private LuhnValidator target = null!;

        [SetUp]
        public void SetUp()
        {
            this.target = new LuhnValidator();
        }

        [TestCase("4111111111111111")]
        [TestCase("4000 0000 0000 0119")]
        [TestCase("4000000000000259")]
        [TestCase("4000 0000 0000 3238")]
        public void IsValid_WhenCorrectCreditCardNumber_ShouldBeTrue(string correctCreditCardNumber)
        {
            var actualt = this.target.IsValid(correctCreditCardNumber);

            Assert.True(actualt);
        }

        [TestCase("411112211111")]
        [TestCase("4000 00119")]
        [TestCase("4123000000259")]
        [TestCase("4000sdf00 3238")]
        [TestCase("This is not a valid one")]
        public void IsValid_WhenIncorrectCreditCardNumber_ShouldBeFalse(string correctCreditCardNumber)
        {
            var actualt = this.target.IsValid(correctCreditCardNumber);

            Assert.False(actualt);
        }
    }
}
