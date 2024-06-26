using System.Collections.Generic;
using AutoMapper;
using FFXIVVenues.Api.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using FFXIVVenues.Api.Security;
using FFXIVVenues.Api.Observability;
using FFXIVVenues.Api.PersistenceModels.Context;
using FFXIVVenues.Api.PersistenceModels.Mapping;
using FFXIVVenues.Api.PersistenceModels.Media;
using FFXIVVenues.VenueModels;

namespace FFXIVVenues.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        var venueCache = new RollingCache<IEnumerable<Venue>>(3*60*1000, 30*60*1000);

        var mediaStorageProvider = configuration.GetValue<string>("MediaStorage:Provider");
        var authorizationKeys = new List<AuthorizationKey>();
        configuration.GetSection("Security:AuthorizationKeys").Bind(authorizationKeys);

        services.AddSingleton<IFFXIVVenuesDbContextFactory, FFXIVVenuesDbContextFactory>();
        if (mediaStorageProvider.ToLower() == "azure")
            services.AddSingleton<IMediaRepository, AzureMediaRepository>();
        else
            services.AddSingleton<IMediaRepository, LocalMediaRepository>();
        services.AddSingleton(venueCache);
        services.AddSingleton<IMapFactory>(new MapFactory(configuration));
        services.AddSingleton<IAuthorizationManager, AuthorizationManager>();
        services.AddSingleton<IChangeBroker, ChangeBroker>();
        services.AddSingleton<IEnumerable<AuthorizationKey>>(authorizationKeys);
        services.AddControllers();
        services.AddHttpContextAccessor();
        services.AddSwaggerGen(c =>
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "FFXIV Venues API", Version = "v1" }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
            
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        if (configuration.GetValue("HttpsOnly", true))
            app.UseHttpsRedirection();

        if (configuration.GetValue("EnableSwagger", true))
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FFXIV Venues API v1"));
        }

        app.UseCors(
                pb => pb.SetIsOriginAllowed(s => true).AllowCredentials().AllowAnyHeader())
            .UseWebSockets()
            .UseRouting()
            .UseEndpoints(endpoints => endpoints.MapControllers());

    }
}