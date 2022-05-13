namespace PaymentGateway.Application.Authorize;

using MediatR;
using PaymentGateway.Application.SharedModels;
using PaymentGateway.Infrastructure;
using PaymentGateway.Repositories;
using PaymentGateway.Repositories.Dtos;
using PaymentGateway.Services;
using PaymentGateway.Services.Models;

public class AuthorizeHandler : IRequestHandler<AuthorizeRequest, AuthorizeResponse>
{
    private readonly IPaymentProcessorService paymentProcessor;
    private readonly IFraudDetectionService fraudDetectionService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<AuthorizeHandler> logger;
    private readonly ICurrentCallerContext currentUserContext;

    public AuthorizeHandler(
        IPaymentProcessorService paymentProcessor,
        IFraudDetectionService fraudDetectionService,
        IUnitOfWork unitOfWork,
        ILogger<AuthorizeHandler> logger,
        ICurrentCallerContext currentUserContext
        )
    {
        this.paymentProcessor = paymentProcessor;
        this.fraudDetectionService = fraudDetectionService;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
        this.currentUserContext = currentUserContext;
    }

    private string? RecognizeIfShouldFail(string creditCardNumber)
    {
        if(creditCardNumber == "4000000000000259")
        {
            return "capture";
        }

        if(creditCardNumber == "4000000000003238")
        {
            return "refund";
        }

        return null;
    }

    public async Task<AuthorizeResponse> Handle(AuthorizeRequest request, CancellationToken cancellationToken)
    {
        var authorization = new AuthorizationDto

        {
            AutorizationId = Guid.NewGuid().ToString("N"),
            Amount = request.Amount,
            AvailableAmount = request.Amount,
            Currency = request.Currency,
            Last4Digits = request.CardNumber[^4..],
            MerchantId = this.currentUserContext.GetCallerId(),

            // This is done only for demo purposes to fail on certain conditions
            ShouldFailOn = this.RecognizeIfShouldFail(request.CardNumber)
        };

        string description;

        // Before we even start asking PaymentProcessor if card can be authorized, lets see in our system if we already have something
        var fraudDetectionResult = await this.fraudDetectionService.CalculateRisk(new FraudDetectionPayload(request.CardHolderName, request.CardNumber, request.ExpiryYear, request.ExpiryMonth, request.CVV, request.Amount, request.Currency));
        if (fraudDetectionResult.Score > 60)
        {
            authorization.Status = AuthorizationStatus.Error;
            description = "Fraud";
        }
        else
        {
            var paymentProcessorAuthorizationResult =
                await this.paymentProcessor.RequestAuthorization(
                        request.CardHolderName,
                        request.CardNumber,
                        request.ExpiryYear,
                        request.ExpiryMonth,
                        request.CVV,
                        request.Amount,
                        request.Currency
                    );

            if (paymentProcessorAuthorizationResult.WasAuthorized)
            {
                authorization.Status = authorization.Status = AuthorizationStatus.Authorized;
                description = "success";
            }
            else
            {
                authorization.Status = authorization.Status = authorization.Status = AuthorizationStatus.Error;
                description = "Refused";
            }

        }

        await this.unitOfWork.AuthorizationRepository.Add(authorization);
        await this.unitOfWork.Complete();

        var status = authorization.Status == AuthorizationStatus.Authorized ? "success" : "error";
        return new AuthorizeResponse(authorization.AutorizationId, new AmountModel(authorization.Amount, authorization.Currency), status, description);
    }
}
