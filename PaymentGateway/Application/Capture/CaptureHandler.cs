namespace PaymentGateway.Application.Capture;

using MediatR;
using PaymentGateway.Application.SharedModels;
using PaymentGateway.Exceptions;
using PaymentGateway.Repositories;
using PaymentGateway.Repositories.Dtos;

public class CaptureHandler : IRequestHandler<CaptureRequest, CaptureResponse>
{
    private readonly IUnitOfWork unitOfWork;

    public CaptureHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<CaptureResponse> Handle(CaptureRequest request, CancellationToken cancellationToken)
    {
        this.unitOfWork.BeginTransaction();
        var authorization = await this.unitOfWork.AuthorizationRepository.Get(request.AuthorizationId);
        if (authorization == null)
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
            captureResponse = new CaptureResponse(new AmountModel(authorization.AvailableAmount, authorization.Currency), "error", "Invalid Authorization status");
        }
        else if (authorization.AvailableAmount < request.Amount)
        {
            captureResponse = new CaptureResponse(new AmountModel(authorization.AvailableAmount, authorization.Currency), "error", "Invalid Amount");
        }
        else
        {
            transaction.Status = TransactionStatus.Succeeded;

            authorization.Status = Repositories.Dtos.AuthorizationStatus.Captured;
            authorization.AvailableAmount = authorization.AvailableAmount - request.Amount;
            this.unitOfWork.AuthorizationRepository.Update(authorization);

            captureResponse = new CaptureResponse(new AmountModel(authorization.AvailableAmount, authorization.Currency), "success", "success");
        }
        
        await this.unitOfWork.TransactionRepository.Add(transaction);
        await this.unitOfWork.Complete();

        return captureResponse;
    }
}
