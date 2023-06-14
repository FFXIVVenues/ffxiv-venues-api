using FFXIVVenues.Api.Persistence;

namespace FFXIVVenues.Api.Security
{
    public interface IAuthorizationCheck
    {
        bool CanNot(Operation op, ISecurityScoped entity = null);

        bool Can(Operation op, ISecurityScoped entity = null);
    }
}