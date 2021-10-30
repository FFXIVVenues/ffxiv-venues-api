namespace FFXIVVenues.Api.Persistence
{
    public interface IEntity
    {
        string Id { get; }

        string OwningKey { get; set; }

    }
}