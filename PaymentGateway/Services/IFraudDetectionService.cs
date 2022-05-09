using PaymentGateway.Services.Models;

namespace PaymentGateway.Services
{
    public interface IFraudDetectionService
    {
        Task<RiskScore> CalculateRisk(FraudDetectionPayload payload);
    }
}
