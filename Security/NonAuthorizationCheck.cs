using FFXIVVenues.Api.Persistence;

namespace FFXIVVenues.Api.Security
{
    public class NonAuthorizationCheck : IAuthorizationCheck
    {

        public bool CanNot(Operation _, ISecurityScoped __ = null) => true;

        public bool Can(Operation _, ISecurityScoped __ = null) => false;

    }
}
