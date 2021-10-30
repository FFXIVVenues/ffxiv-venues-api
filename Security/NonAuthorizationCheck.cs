using FFXIVVenues.Api.Persistence;

namespace FFXIVVenues.Api.Security
{
    public class NonAuthorizationCheck : IAuthorizationCheck
    {

        public bool CanNot(Operation _, IEntity __ = null) => true;

        public bool Can(Operation _, IEntity __ = null) => false;

    }
}
