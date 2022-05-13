namespace PaymentGateway.Repositories.Dtos;

using System.ComponentModel.DataAnnotations;

public class AuthorizationDto
{
    [Key]
    public string AutorizationId { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public decimal AvailableAmount { get; set; }

    [Required]
    public string Currency { get; set; } = null!;

    [Required]
    public string Last4Digits { get; set; } = null!;

    [Required]
    public AuthorizationStatus Status { get; set; }

    [Required]
    public string MerchantId { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? ShouldFailOn { get; set; } // this is only for demo purposes
}