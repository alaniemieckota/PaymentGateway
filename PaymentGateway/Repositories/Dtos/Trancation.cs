namespace PaymentGateway.Repositories.Dtos;

using System.ComponentModel.DataAnnotations;

public class TransactionDto
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public AuthorizationDto Authorization { get; set; } = null!;

    public decimal Amount { get; set; }

    public TransactionStatus Status { get; set; }

    public TransactionType Type { get; set; }

    public string? Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
