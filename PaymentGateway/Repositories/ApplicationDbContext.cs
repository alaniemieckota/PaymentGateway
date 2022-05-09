namespace PaymentGateway.Repositories;

using Microsoft.EntityFrameworkCore;
using PaymentGateway.Repositories.Dtos;

public class ApplicationDbContext : DbContext
{
    public DbSet<AutorizationDto> Authorizations { get; set; } = null!;
    
    public DbSet<TransactionDto> Transactions { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {         
    }
}
