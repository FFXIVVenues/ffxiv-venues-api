using System;
using System.Linq;
using FFXIVVenues.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var environment = args.SkipWhile(s => !string.Equals(s, "--environment", StringComparison.OrdinalIgnoreCase)).Skip(1).FirstOrDefault()
                                ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                ?? Environments.Production;

var config = new ConfigurationBuilder();
config.AddJsonFile("appsettings.json", optional: true)
        .AddEnvironmentVariables("FFXIVVENUES_")
        .AddUserSecrets<Program>()
        .AddCommandLine(args);

await Host.CreateDefaultBuilder()
    .ConfigureWebHostDefaults(wb => 
        wb.UseKestrel()
            .UseIIS()
            .UseIISIntegration()
            .UseConfiguration(config.Build())
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
    
    .Build().RunAsync();

