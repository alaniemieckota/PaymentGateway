namespace PaymentGatewayTests.Application.Capture;

using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using PaymentGateway.Application.Capture;
using System.Threading;
using PaymentGateway.Repositories;
using PaymentGateway.Infrastructure;
using PaymentGateway.Repositories.Dtos;
using PaymentGateway.Exceptions;

public class CaptureHandlerTests
{
    private readonly CancellationToken cancellationToken = new CancellationToken();

    private const string Currency = "USD";

    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public async Task Handle_WhenCapturingLessThanAuthorized_ShouldCapture()
    {
        // Arrange
        var amountToCapture = 100m;
        var amountAuthorized = 100.99m;
        var expectedAmountLeft = amountAuthorized - amountToCapture;
        var expectedStatus = "success";
        var merchantId = System.Guid.NewGuid().ToString();
        var authorizationId = System.Guid.NewGuid().ToString();
        var request = new CaptureRequest(authorizationId, amountToCapture);
        
        var currentCallerContextMoq = new Mock<ICurrentCallerContext>();
        currentCallerContextMoq.Setup(x => x.GetCallerId()).Returns(merchantId);

        var transactionRepositoryMoq = new Mock<ITransactionRepository>();

        var authorizationRepositoryMoq = new Mock<IAuthorizationRepository>();
        authorizationRepositoryMoq
            .Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync(this.CreateAuthorization(authorizationId, merchantId, amount: amountAuthorized));
        
        var unitOfWorkMoq = new Mock<IUnitOfWork>();
        unitOfWorkMoq.SetupGet(x => x.AuthorizationRepository).Returns(authorizationRepositoryMoq.Object);
        unitOfWorkMoq.SetupGet(x => x.TransactionRepository).Returns(transactionRepositoryMoq.Object);

        var target = new CaptureHandler(unitOfWorkMoq.Object, currentCallerContextMoq.Object);

        // Act
        var actual = await target.Handle(request, this.cancellationToken);

        // Assert
        Assert.NotNull(actual);
        Assert.AreEqual(expectedStatus, actual.Status);
        Assert.AreEqual(expectedAmountLeft, actual.Amount.Amount);
        Assert.AreEqual(Currency, actual.Amount.currency);
    }

    [Test]
    public void Handle_AuthorizationNotFound_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var amountToCapture = 100m;
        var merchantId = System.Guid.NewGuid().ToString();
        var authorizationId = System.Guid.NewGuid().ToString();
        var request = new CaptureRequest(authorizationId, amountToCapture);

        var currentCallerContextMoq = new Mock<ICurrentCallerContext>();
        currentCallerContextMoq.Setup(x => x.GetCallerId()).Returns(merchantId);

        var authorizationRepositoryMoq = new Mock<IAuthorizationRepository>();
        authorizationRepositoryMoq
            .Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync(this.CreateAuthorization(authorizationId, "dummyAuthroizationId"));

        var unitOfWorkMoq = new Mock<IUnitOfWork>();
        unitOfWorkMoq.SetupGet(x => x.AuthorizationRepository).Returns(authorizationRepositoryMoq.Object);
        var target = new CaptureHandler(unitOfWorkMoq.Object, currentCallerContextMoq.Object);

        // Act
        var exception = Assert.ThrowsAsync<EntityNotFoundException>(async () => await target.Handle(request, this.cancellationToken));

        // Assert
        Assert.True(exception!.Message.Contains(authorizationId));
    }

    [Test]
    public async Task Handle_WhenCapturingMoreAuthorized_ShouldFailCapturing()
    {
        // Arrange
        var amountToCapture = 200m;
        var amountAuthorized = 100.99m;
        
        var expectedAmountLeft = amountAuthorized;
        var expectedStatus = "error";
        var expectedDescription = "Invalid Amount";

        var merchantId = System.Guid.NewGuid().ToString();
        var authorizationId = System.Guid.NewGuid().ToString();
        var request = new CaptureRequest(authorizationId, amountToCapture);

        var currentCallerContextMoq = new Mock<ICurrentCallerContext>();
        currentCallerContextMoq.Setup(x => x.GetCallerId()).Returns(merchantId);

        var transactionRepositoryMoq = new Mock<ITransactionRepository>();

        var authorizationRepositoryMoq = new Mock<IAuthorizationRepository>();
        authorizationRepositoryMoq
            .Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync(this.CreateAuthorization(authorizationId, merchantId, amount: amountAuthorized));

        var unitOfWorkMoq = new Mock<IUnitOfWork>();
        unitOfWorkMoq.SetupGet(x => x.AuthorizationRepository).Returns(authorizationRepositoryMoq.Object);
        unitOfWorkMoq.SetupGet(x => x.TransactionRepository).Returns(transactionRepositoryMoq.Object);

        var target = new CaptureHandler(unitOfWorkMoq.Object, currentCallerContextMoq.Object);

        // Act
        var actual = await target.Handle(request, this.cancellationToken);

        // Assert
        Assert.NotNull(actual);
        Assert.AreEqual(expectedStatus, actual.Status);
        Assert.AreEqual(expectedAmountLeft, actual.Amount.Amount);
        Assert.AreEqual(Currency, actual.Amount.currency);
        Assert.AreEqual(expectedDescription, actual.Description);
    }

    [Test]
    public async Task Handle_WhenCapturingVoidedAuthorization_ShouldFailCapturing()
    {
        // Arrange
        var amountToCapture = 100.99m;
        var amountAuthorized = 100.99m;

        var expectedStatus = "error";
        var expectedDescription = "Invalid Authorization status";

        var merchantId = System.Guid.NewGuid().ToString();
        var authorizationId = System.Guid.NewGuid().ToString();
        var request = new CaptureRequest(authorizationId, amountToCapture);

        var currentCallerContextMoq = new Mock<ICurrentCallerContext>();
        currentCallerContextMoq.Setup(x => x.GetCallerId()).Returns(merchantId);

        var transactionRepositoryMoq = new Mock<ITransactionRepository>();

        var authorizationRepositoryMoq = new Mock<IAuthorizationRepository>();
        authorizationRepositoryMoq
            .Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync(this.CreateAuthorization(authorizationId, merchantId, amount: amountAuthorized, status: AuthorizationStatus.Voided));

        var unitOfWorkMoq = new Mock<IUnitOfWork>();
        unitOfWorkMoq.SetupGet(x => x.AuthorizationRepository).Returns(authorizationRepositoryMoq.Object);
        unitOfWorkMoq.SetupGet(x => x.TransactionRepository).Returns(transactionRepositoryMoq.Object);

        var target = new CaptureHandler(unitOfWorkMoq.Object, currentCallerContextMoq.Object);

        // Act
        var actual = await target.Handle(request, this.cancellationToken);

        // Assert
        Assert.NotNull(actual);
        Assert.AreEqual(expectedStatus, actual.Status);
        Assert.AreEqual(expectedDescription, actual.Description);
    }

    private AuthorizationDto CreateAuthorization(
        string authorizationId,
        string merchantId,
        decimal amount = 100.99m,
        decimal availableAmount = 100.99m,
        AuthorizationStatus status = AuthorizationStatus.Authorized,
        string currency = Currency,
        string last4Digits = "1234"
        )
    {
        return new AuthorizationDto
        {
            AutorizationId = authorizationId,
            Amount = amount,
            AvailableAmount = availableAmount,
            MerchantId = merchantId,
            Status = status,
            CreatedAt = System.DateTime.UtcNow.AddDays(-1),
            Currency = currency,
            Last4Digits = last4Digits
        };
    }
}
