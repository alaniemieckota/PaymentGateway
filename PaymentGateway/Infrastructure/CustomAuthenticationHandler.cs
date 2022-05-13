namespace PaymentGateway.Infrastructure;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationSchemaOptions>
{
    public const string HeaderKeyNameXKey = "X-Key";

    public const string SchemaName = "X-Key";

    public CustomAuthenticationHandler(
        IOptionsMonitor<CustomAuthenticationSchemaOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // simple logic to check if key in header exists, for demo purposes this is enough to say that caller is authenticated
        if(!Request.Headers.ContainsKey(HeaderKeyNameXKey))
        {
            return Task.FromResult(AuthenticateResult.Fail($"Header Not Found. Should be: {CurrentcallerContext.HeaderKeyNameXKey}"));
        }

        var key = Request.Headers[HeaderKeyNameXKey][0];

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, key), new Claim(ClaimTypes.Name, key) };
        var claimsIdentity = new ClaimsIdentity(claims, nameof(CustomAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);

        // pass on the ticket to the middleware
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
