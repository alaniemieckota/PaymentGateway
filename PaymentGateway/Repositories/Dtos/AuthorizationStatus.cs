namespace PaymentGateway.Repositories.Dtos;

public enum AuthorizationStatus
{
    Error,
    Authorized,
    Captured,
    Voided,
    Refunded
}
