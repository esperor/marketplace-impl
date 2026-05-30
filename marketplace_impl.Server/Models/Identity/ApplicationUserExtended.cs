using marketplace_impl.Server.Configs.Enums;
using marketplace_impl.Server.Data;
using System.Diagnostics.CodeAnalysis;

namespace marketplace_impl.Server.Models.Identity
{
    public class ApplicationUserExtended : ApplicationUser
    {
        public required EAccessTrait AccessTraits { get; init; }

        [SetsRequiredMembers]
        public ApplicationUserExtended(ApplicationUser user, EAccessTrait accessTraits)
            : base(user)
        {
            AccessTraits = accessTraits;
        }
    }
}
