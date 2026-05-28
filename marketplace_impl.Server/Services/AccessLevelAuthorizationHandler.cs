using course.Server.Configs;
using course.Server.Configs.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Security.Claims;

namespace course.Server.Services
{
    public class AccessLevelAuthorizationHandler : AuthorizationHandler<AuthorizeAccessLevelAttribute>
    {
        private readonly IdentityService _identityService;

        public AccessLevelAuthorizationHandler(IdentityService identityService)
        {
            _identityService = identityService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuthorizeAccessLevelAttribute requirement)
        {
            if (requirement == null) return;

            if (context.Resource is not HttpContext httpContext)
                throw new Exception("No http context in authorization attribute");

            var user = await _identityService.GetUser(httpContext);
            if (user == null) return;

            if (user.GetAccessLevel() >= requirement.AccessLevel) 
                context.Succeed(requirement);
  
            return;
        }
    }
}
