namespace FFXIVVenues.Api.Security.ServiceAuthentication
{
    public interface ISecurityScoped
    {
        bool Approved { get; }
        string ScopeKey { get; }
    }
}