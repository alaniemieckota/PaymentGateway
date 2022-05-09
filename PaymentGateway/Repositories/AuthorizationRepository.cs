namespace PaymentGateway.Repositories;

using PaymentGateway.Repositories.Dtos;

public class AuthorizationRepository : GenericRepository<AutorizationDto>, IAuthorizationRepository
{
    public AuthorizationRepository(ApplicationDbContext context) : base(context)
    {
    }
}
