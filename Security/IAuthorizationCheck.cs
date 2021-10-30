using FFXIVVenues.Api.Persistence;

namespace FFXIVVenues.Api.Security
{
    public interface IAuthorizationCheck
    {
        bool CanNot(Operation op, IEntity entity = null);

        bool Can(Operation op, IEntity entity = null);

    }
}