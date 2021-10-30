namespace FFXIVVenues.Api.Security
{
    public interface IAuthorizationManager
    {
        string GetKey();
        IAuthorizationCheck Check(string key = null);
    }
}