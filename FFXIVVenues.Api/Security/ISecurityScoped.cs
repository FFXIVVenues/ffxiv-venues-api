namespace FFXIVVenues.Api.Security
{
    public interface ISecurityScoped
    {
        bool Approved { get; }
        string ScopeKey { get; }
    }
}