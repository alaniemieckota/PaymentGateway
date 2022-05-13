namespace PaymentGateway.Application.Refund;

using MediatR;
using PaymentGateway.Application.SharedModels;
using PaymentGateway.Exceptions;
using PaymentGateway.Infrastructure;
using PaymentGateway.Repositories;
using PaymentGateway.Repositories.Dtos;
using System.Threading;
using System.Threading.Tasks;

public class RefundHandler : IRequestHandler<RefundRequest, RefundResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrentCallerContext currentUserContext;

    public RefundHandler(IUnitOfWork unitOfWork, ICurrentCallerContext currentUserContext)
    {
        this.unitOfWork = unitOfWork;
        this.currentUserContext = currentUserContext;
    }

    public async Task<RefundResponse> Handle(RefundRequest request, CancellationToken cancellationToken)
    {
        this.unitOfWork.BeginTransaction();
        var authorization = await this.unitOfWork.AuthorizationRepository.Get(request.AuthorizationId);

        if (authorization == null
            || authorization.MerchantId != this.currentUserContext.GetCallerId())
        {
            throw new EntityNotFoundException(request.AuthorizationId);
        }

        var amountAvailableToRefund = authorization.Amount - authorization.AvailableAmount;
        var transaction = new TransactionDto
        {
            Amount = request.Amount,
            Authorization = authorization,
            Type = TransactionType.Refund
        };

        RefundResponse refundResponse;
        if (
            authorization.Status != Repositories.Dtos.AuthorizationStatus.Captured
            && authorization.Status != Repositories.Dtos.AuthorizationStatus.Refunded
            )
        {
            var description = "Invalid Authroization status";

            transaction.Description = description;
            transaction.Status = TransactionStatus.Error;

            refundResponse = new RefundResponse(new AmountModel(amountAvailableToRefund, authorization.Currency), "error", description);
        }
        else if (request.Amount > amountAvailableToRefund)
        {
            var description = "Invalid Amount";

            transaction.Description = description;
            transaction.Status = TransactionStatus.Error;

            refundResponse = new RefundResponse(new AmountModel(amountAvailableToRefund, authorization.Currency), "error", description);
        }
        else
        {
            amountAvailableToRefund = amountAvailableToRefund - request.Amount;

            authorization.AvailableAmount = authorization.AvailableAmount + request.Amount;
            transaction.Status = TransactionStatus.Succeeded;

            refundResponse = new RefundResponse(new AmountModel(amountAvailableToRefund, authorization.Currency), "success", "success");
        }

        await this.unitOfWork.TransactionRepository.Add(transaction);
        await this.unitOfWork.Complete();
        
        return refundResponse;
    }
}
