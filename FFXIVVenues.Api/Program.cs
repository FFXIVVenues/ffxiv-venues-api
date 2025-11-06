using System;
using System.Linq;
using FFXIVVenues.Api;
using FFXIVVenues.DomainData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var environment = args.SkipWhile(s => !string.Equals(s, "--environment", StringComparison.OrdinalIgnoreCase)).Skip(1).FirstOrDefault()
                  ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environments.Production;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables("FFXIV_VENUES_API:")
    .AddUserSecrets<Program>()
    .AddCommandLine(args)
    .Build();

var host = Host.CreateDefaultBuilder()
    .ConfigureWebHostDefaults(wb => 
        wb.UseKestrel()
            .UseIIS()
            .UseIISIntegration()
            .UseConfiguration(config)
            .UseStartup<Startup>())
    .UseEnvironment(environment)
    .ConfigureLogging((context, logging) =>
    {
        var loggingConfig = context.Configuration.GetSection("Logging");
        logging.AddConfiguration(loggingConfig);
        logging.AddConsole();
        logging.AddDebug();
        logging.AddEventSourceLogger();
    })
    .Build();

await host.Services.MigrateDomainDataAsync();

await host.RunAsync();