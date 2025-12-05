using FFXIVVenues.DomainData;
using FFXIVVenues.FlagService.Client.Commands;
using Serilog;
using Serilog.Events;
using Wolverine;
using Wolverine.Pulsar;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables("FFXIV_VENUES_FLAGSERVICE__")
    .AddUserSecrets<Program>()
    .AddCommandLine(args)
    .Build();

var connectionString = config.GetConnectionString("FFXIVVenues");
var mediaUriTemplate = config.GetValue<string>("MediaStorage:UriTemplate");
var betterStackToken = config.GetValue<string>("Logging:BetterStackToken");
var minLevel = config.GetValue<LogEventLevel>("Logging:MinimumLevel");
var pulsarServiceUrl = config.GetValue<string>("Pulsar:ServiceUrl");
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.BetterStack(betterStackToken)
    .MinimumLevel.Is(minLevel)
    .Destructure.ByTransforming<FFXIVVenues.VenueModels.Venue>(
        v => new { VenueId = v.Id, VenueName = v.Name })
    .Destructure.ByTransforming<FFXIVVenues.DomainData.Entities.Venues.Venue>(
        v => new { VenueId = v.Id, VenueName = v.Name })
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDomainData(connectionString, mediaUriTemplate);
builder.Logging.AddSerilog();
builder.UseWolverine(opts =>
{
    opts.UsePulsar(pulsar =>
        pulsar.ServiceUrl(new Uri(pulsarServiceUrl)));

    opts.PublishMessage<FlagVenueCommand>()
        .ToPulsarTopic("persistent://ffxivvenues/flagging/commands");

    opts.ListenToPulsarTopic("persistent://ffxivvenues/flagging/commands")
        .ProcessInline();
});


var host = builder.Build();

await host.Services.MigrateDomainDataAsync();
await host.RunAsync();