namespace PaymentGateway.Services;

using PaymentGateway.Services.Models;

public class DummyFraudDetectionService : IFraudDetectionService
{
    public async Task<RiskScore> CalculateRisk(FraudDetectionPayload payload)
    {
        var random = new Random(Guid.NewGuid().GetHashCode());
        await Task.Delay(random.Next(1000)); // simulate response delay

        return new RiskScore(10, "Ok"); // this is just a dummy reponse
    }
}
