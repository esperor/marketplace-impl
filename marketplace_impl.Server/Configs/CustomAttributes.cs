using course.Server.Configs.Enums;
using Microsoft.AspNetCore.Authorization;

namespace course.Server.Configs
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
