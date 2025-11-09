using FFXIVVenues.DomainData;
using FFXIVVenues.DomainData.Context;
using FFXIVVenues.DomainData.Mapping;
using FFXIVVenues.OGCard;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables("FFXIV_VENUES_OGCARD__")
    .AddUserSecrets<Program>()
    .AddCommandLine(args)
    .Build();

Console.WriteLine("All keys in config:");
foreach (var kv in config.AsEnumerable())
    Console.WriteLine($"{kv.Key} = {kv.Value}");

Console.WriteLine("Connection string lookup:");
Console.WriteLine(config.GetConnectionString("FFXIVVenues") ?? "<null>");

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