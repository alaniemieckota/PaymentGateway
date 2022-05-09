namespace PaymentGateway.Repositories.Dtos;

using System.ComponentModel.DataAnnotations;

public class AutorizationDto
{
    [Key]
    public string AutorizationId { get; set; } = null!;
    
    public decimal Amount { get; set; }

    public decimal AvailableAmount { get; set; }

    public string Currency { get; set; } = null!;

    public string Last4Digits { get; set; } = null!;

    public AuthorizationStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}