namespace PaymentGateway.Services;

public class DummyIdempotencyService : IIdempotencyService
{
    public async Task CheckUniqness(string idempotencyKey)
    {
        // This should be a call to external resource/cache/service which holds information if given 
        // key is unique, if not then application should throw 409 (Conflict) 

        var random = new Random(Guid.NewGuid().GetHashCode());
        await Task.Delay(random.Next(10)); // simulate response delay
    }
}
