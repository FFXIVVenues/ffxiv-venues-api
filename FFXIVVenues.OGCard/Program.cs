using FFXIVVenues.DomainData;
using FFXIVVenues.DomainData.Context;
using FFXIVVenues.DomainData.Mapping;
using FFXIVVenues.OGCard;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables("FFXIV_VENUES_API:")
    .AddUserSecrets<Program>()
    .AddCommandLine(args)
    .Build();

var connectionString = config.GetConnectionString("FFXIVVenues");
var mediaUriTemplate = config.GetValue<string>("UriTemplate", 
    "https://images.ffxivvenues.dev/{venueId}/{bannerKey}");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDomainData(connectionString, mediaUriTemplate);
var app = builder.Build();
app.MapGet("/venue/{venueId}", (string venueId, IMapFactory mapFactory, DomainDataContext domainData) =>
{
    var query = domainData.Venues.AsQueryable().Where(v => v.Id == venueId);
    var venue = mapFactory.GetModelProjector().ProjectTo<FFXIVVenues.VenueModels.Venue>(query).SingleOrDefault();
    if (venue is null)
        return Results.NotFound();
    
    var template = new OGCardTemplate();
    template.Session = new Dictionary<string, object>();
    template.Session["venue"] = venue;
    return Results.Content(template.TransformText(), "text/html");
});
await app.RunAsync();