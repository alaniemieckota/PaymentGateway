namespace PaymentGateway.Exceptions;

using System.Runtime.Serialization;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string id) : base($"Entity {id} not found.")
    {

    }

    protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
