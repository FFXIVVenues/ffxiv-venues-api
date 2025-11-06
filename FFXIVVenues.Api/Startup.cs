using System.Collections.Generic;
using System.Reflection;
using FFXIVVenues.Api.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using FFXIVVenues.Api.Security;
using FFXIVVenues.Api.Observability;
using FFXIVVenues.Api.Media;
using FFXIVVenues.DomainData;
using FFXIVVenues.DomainData.Context;
using FFXIVVenues.DomainData.Mapping;
using FFXIVVenues.VenueModels;
using Scalar.AspNetCore;

namespace FFXIVVenues.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        var venueCache = new RollingCache<IEnumerable<Venue>>(3*60*1000, 30*60*1000);

        var connectionString = configuration.GetConnectionString("FFXIVVenues");
        var mediaUriTemplate = configuration.GetValue<string>("MediaStorage:UriTemplate");
        var mediaStorageProvider = configuration.GetValue<string>("MediaStorage:Provider");
        var authorizationKeys = new List<AuthorizationKey>();
        configuration.GetSection("Security:AuthorizationKeys").Bind(authorizationKeys);

        if (mediaStorageProvider.ToLower() == "s3")
            services.AddSingleton<IMediaRepository, S3MediaRepository>();
        else if (mediaStorageProvider.ToLower() == "azure")
            services.AddSingleton<IMediaRepository, AzureMediaRepository>();
        else
            services.AddSingleton<IMediaRepository, LocalMediaRepository>();
        services.AddSingleton(venueCache);
        
        // todo: move these to UseDomainData() or something
        services.AddDomainData(connectionString, mediaUriTemplate);
        
        services.AddSingleton<IAuthorizationManager, AuthorizationManager>();
        services.AddSingleton<IChangeBroker, ChangeBroker>();
        services.AddSingleton<IEnumerable<AuthorizationKey>>(authorizationKeys);
        services.AddControllers();
        services.AddHttpContextAccessor();
        
        // todo: Remove the below services for .net 9
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "FFXIV Venues API", Version = "v1" });
            c.IncludeXmlComments(Assembly.GetExecutingAssembly());
        });
        services.AddEndpointsApiExplorer();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
            
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        if (configuration.GetValue("HttpsOnly", true))
            app.UseHttpsRedirection();

        app.UseCors(
                pb => pb.SetIsOriginAllowed(_ => true).AllowCredentials().AllowAnyHeader())
            .UseSwagger(options => 
                options.RouteTemplate = "openapi/{documentName}.json")
            .UseWebSockets()
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapScalarApiReference();
                endpoints.MapControllers();
            });

        // todo: Add for .net 9, replace UseSwagger and MapScalarApiReference
        // app.MapOpenApi();
        // app.MapScalarApiReference();
    }
}