namespace PaymentGateway.Exceptions;

using System.Runtime.Serialization;

public class RequestNotUniqueException : Exception
{
    public RequestNotUniqueException(string key) : base($"Duplicate request with idempotency key: {key}")
    {

    }

    protected RequestNotUniqueException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}