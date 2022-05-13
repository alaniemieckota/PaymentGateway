namespace PaymentGateway.Application.Capture;

using MediatR;
using PaymentGateway.Application.SharedModels;
using PaymentGateway.Exceptions;
using PaymentGateway.Infrastructure;
using PaymentGateway.Repositories;
using PaymentGateway.Repositories.Dtos;

public class CaptureHandler : IRequestHandler<CaptureRequest, CaptureResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrentCallerContext currentUserContext;

    public CaptureHandler(IUnitOfWork unitOfWork, ICurrentCallerContext currentUserContext)
    {
        this.unitOfWork = unitOfWork;
        this.currentUserContext = currentUserContext;
    }

    public async Task<CaptureResponse> Handle(CaptureRequest request, CancellationToken cancellationToken)
    {
        this.unitOfWork.BeginTransaction();
        var authorization = await this.unitOfWork.AuthorizationRepository.Get(request.AuthorizationId);
        if (authorization == null 
            || authorization.MerchantId != this.currentUserContext.GetCallerId())
        {
            throw new EntityNotFoundException(request.AuthorizationId);
        }

        var transaction = new TransactionDto
        {
            Amount = request.Amount,
            Authorization = authorization,
            Type = TransactionType.Capture
        };

        CaptureResponse captureResponse;

        if (
            authorization.Status != AuthorizationStatus.Authorized 
            && authorization.Status != AuthorizationStatus.Captured
            )
        {
            var description = "Invalid Authorization status";
            
            transaction.Status = TransactionStatus.Error;
            transaction.Description = description;

            captureResponse = new CaptureResponse(new AmountModel(authorization.AvailableAmount, authorization.Currency), "error", description);
        }
        else if (authorization.AvailableAmount < request.Amount)
        {
            var description = "Invalid Amount";

            transaction.Status = TransactionStatus.Error;
            transaction.Description = description;

            captureResponse = new CaptureResponse(new AmountModel(authorization.AvailableAmount, authorization.Currency), "error", description);
        }
        else if(authorization.ShouldFailOn == "capture")
        {
            var description = "Capture failed because this is a demo cptured failed credit card";

            transaction.Status = TransactionStatus.Error;
            transaction.Description = description;

            captureResponse = new CaptureResponse(new AmountModel(authorization.AvailableAmount, authorization.Currency), "error", description);
        }
        else
        {
            transaction.Status = TransactionStatus.Succeeded;

            authorization.Status = Repositories.Dtos.AuthorizationStatus.Captured;
            authorization.AvailableAmount -= request.Amount;
            this.unitOfWork.AuthorizationRepository.Update(authorization);

            captureResponse = new CaptureResponse(new AmountModel(authorization.AvailableAmount, authorization.Currency), "success", "success");
        }
        
        await this.unitOfWork.TransactionRepository.Add(transaction);
        await this.unitOfWork.Complete();

        return captureResponse;
    }
}
