namespace PaymentGateway.Application.Void;

using MediatR;
using PaymentGateway.Application.SharedModels;
using PaymentGateway.Exceptions;
using PaymentGateway.Repositories;
using PaymentGateway.Repositories.Dtos;
using System.Threading;
using System.Threading.Tasks;

public class VoidHandler : IRequestHandler<VoidRequest, VoidResponse>
{
    private readonly IUnitOfWork unitOfWork;

    public VoidHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<VoidResponse> Handle(VoidRequest request, CancellationToken cancellationToken)
    {
        this.unitOfWork.BeginTransaction();
        var authorization = await this.unitOfWork.AuthorizationRepository.Get(request.AuthorizationId);
        if (authorization == null)
        {
            throw new EntityNotFoundException(request.AuthorizationId);
        }

        var transaction = new TransactionDto
        {
            Authorization = authorization,
            Type = TransactionType.Void
        };

        VoidResponse voidResponse;
        if(authorization.Status != Repositories.Dtos.AuthorizationStatus.Authorized)
        {
            var description = "Invalida Authorization status";
            
            transaction.Description = description;
            voidResponse = new VoidResponse(new AmountModel(authorization.AvailableAmount, authorization.Currency), "error", description);
        }
        else
        {
            authorization.Status = Repositories.Dtos.AuthorizationStatus.Voided;
            this.unitOfWork.AuthorizationRepository.Update(authorization);
            voidResponse = new VoidResponse(new AmountModel(authorization.Amount, authorization.Currency), "success", "success");
        }

        await this.unitOfWork.TransactionRepository.Add(transaction);
        await this.unitOfWork.Complete();

        return voidResponse;
    }
}
