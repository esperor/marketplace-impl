using marketplace_impl.Server.Configs.Enums;
using marketplace_impl.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace marketplace_impl.Server.Configs.Authentication
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public IServiceProvider ServiceProvider { get; set; }

        [Obsolete]
        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock, 
            IServiceProvider serviceProvider
            )
            : base(options, logger, encoder, clock)
        {
            ServiceProvider = serviceProvider;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identityService = ServiceProvider.GetRequiredService<IdentityService>();

            var user = await identityService.GetUser(Request.HttpContext);
            if (user is null) return AuthenticateResult.Fail("Authentication cookie not found");

            var claims = Enum.GetValues<EAccessTrait>().Select(accessTrait =>
                new Claim(
                    accessTrait.ToString(),
                    user.AccessTraits.HasFlag(accessTrait).ToString())
            ).ToArray();

            var identity = new ClaimsIdentity(claims, nameof(AuthenticationHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
