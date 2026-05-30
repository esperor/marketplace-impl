using Microsoft.AspNetCore.Authorization;

namespace marketplace_impl.Server.Configs.Authorization
{
    public class AccessTraitAuthorizationHandler : AuthorizationHandler<AuthorizeAccessTraitAttribute>
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuthorizeAccessTraitAttribute requirement)
        {
            if (requirement == null || context.User.Identity?.IsAuthenticated != true) return;

            foreach (var accessTrait in requirement.AccessTraits)
            {
                if (!context.User.Claims.Any(claim =>
                        claim.Type == accessTrait.ToString() && claim.Value == true.ToString()))
                    return;
            }
            
            context.Succeed(requirement);
  
            return;
        }
    }
}
