namespace PaymentGateway.Repositories;

using PaymentGateway.Repositories.Dtos;

public class AuthorizationRepository : GenericRepository<AuthorizationDto>, IAuthorizationRepository
{
    public AuthorizationRepository(ApplicationDbContext context) : base(context)
    {
    }
}
