using System.Collections.Generic;
using FFXIVVenues.Api.Helpers;
using FFXIVVenues.Api.InternalModel.Marshalling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using FFXIVVenues.Api.Persistence;
using FFXIVVenues.Api.Security;
using FFXIVVenues.Api.Observability;
using FFXIVVenues.VenueModels;

namespace FFXIVVenues.Api
{
    public class Startup
    {

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var venueCache = new RollingCache<IEnumerable<Venue>>(3*60*1000, 30*60*1000);

            var connectionString = _configuration.GetValue<string>("Persistence:ConnectionString");
            var mediaStorageProvider = _configuration.GetValue<string>("MediaStorage:Provider");
            var authorizationKeys = new List<AuthorizationKey>();
            _configuration.GetSection("Security:AuthorizationKeys").Bind(authorizationKeys);

            services.AddSingleton<IObjectRepository>(new LiteDbRepository(connectionString));
            if (mediaStorageProvider.ToLower() == "azure")
                services.AddSingleton<IMediaRepository, AzureMediaRepository>();
            else
                services.AddSingleton<IMediaRepository, LocalMediaRepository>();
            services.AddSingleton(venueCache);
            services.AddSingleton<IAuthorizationManager, AuthorizationManager>();
            services.AddSingleton<IChangeBroker, ChangeBroker>();
            services.AddSingleton<ISplashUriBuilder, SplashUriBuilder>();
            services.AddSingleton<IModelMarshaller, ModelMarshaller>();
            services.AddSingleton<IEnumerable<AuthorizationKey>>(authorizationKeys);
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FFXIV Venues API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var authorizationKey = _configuration.GetValue<string>("Security:AuthorizationKey");

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            if (_configuration.GetValue("HttpsOnly", true))
                app.UseHttpsRedirection();

            if (_configuration.GetValue("EnableSwagger", true))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FFXIV Venues API v1"));
            }

            app.UseCors(policyBuilder => policyBuilder.SetIsOriginAllowed(s => true).AllowCredentials().AllowAnyHeader())
               .UseRouting();

            if (!string.IsNullOrEmpty(authorizationKey))
            {
                app.Use((context, n) =>
                {
                    if (context.Request.Headers["Authorization"] == $"Bearer {authorizationKey}")
                        context.Items.Add("authorized", true);
                    return n();
                });
            }

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
