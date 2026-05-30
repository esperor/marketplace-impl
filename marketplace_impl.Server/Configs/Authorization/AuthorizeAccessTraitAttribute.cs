using marketplace_impl.Server.Configs.Enums;
using Microsoft.AspNetCore.Authorization;

namespace marketplace_impl.Server.Configs.Authorization
{
    public class AuthorizeAccessTraitAttribute
        : AuthorizeAttribute
        , IAuthorizationRequirement
        , IAuthorizationRequirementData
    {
        public EAccessTrait[] AccessTraits { get; }

        public AuthorizeAccessTraitAttribute(params EAccessTrait[] accessTraits) => AccessTraits = accessTraits;

        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return this;
        }
    }
}
