namespace FFXIVVenues.Api.Persistence
{
    public interface IEntity
    {
        string Id { get; }
    }
    
    public interface ISecurityScoped
    {
        string ScopeKey { get; }
    }
}