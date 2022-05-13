namespace PaymentGateway.Infrastructure;


public class CurrentcallerContext : ICurrentCallerContext
{
    public const string HeaderKeyNameXKey = "X-Key";
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentcallerContext(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string GetCallerId()
    {
        if(this.httpContextAccessor.HttpContext == null 
            || this.httpContextAccessor.HttpContext.User == null)
        {
            throw new InvalidOperationException("Httpcontext or user missing");
        }

        return this.httpContextAccessor.HttpContext.User.Identity!.Name!;
    }
}
