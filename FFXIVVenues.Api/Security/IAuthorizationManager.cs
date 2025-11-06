namespace FFXIVVenues.Api.Security;

public interface IAuthorizationManager
{
    string GetKeyString();
    AuthorizationKey GetKey(string key = null);
    bool IsAuthenticated();
    IAuthorizationCheck Check(string key = null);
}