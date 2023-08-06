using System.Linq;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;

namespace FFXIVVenues.Api.Security
{
    public interface IAuthorizationManager
    {
        string GetKeyString();
        AuthorizationKey GetKey(string key = null);
        bool IsAuthenticated();
        IAuthorizationCheck Check(string key = null);
    }
}