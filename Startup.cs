using System.Collections.Generic;
using System.Net.Http;
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
using FFXIVVenues.Api.Security.ServiceAuthentication;
using FFXIVVenues.Api.Security.UserAuthentication;
using FFXIVVenues.VenueModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using User = Sentry.User;

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

            var mediaStorageProvider = _configuration.GetValue<string>("MediaStorage:Provider");
            var discordClientId = _configuration.GetValue<string>("DiscordApi:ClientId");
            var discordClientSecret = _configuration.GetValue<string>("DiscordApi:ClientSecret");
            var authorizationKeys = new List<AuthorizationKey>();
            _configuration.GetSection("Security:AuthorizationKeys").Bind(authorizationKeys);

            services.AddSingleton<IFFXIVVenuesDbContextFactory, FFXIVVenuesDbContextFactory>();
            if (mediaStorageProvider.ToLower() == "azure")
                services.AddSingleton<IMediaRepository, AzureMediaRepository>();
            else
                services.AddSingleton<IMediaRepository, LocalMediaRepository>();
            
            services
                .AddAuthentication(DiscordDefaults.AuthenticationScheme)
                .AddCookie()
                .AddDiscord(x => {
                    x.ClientId = discordClientId;
                    x.ClientSecret = discordClientSecret;
                    x.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });

            services.AddSingleton(venueCache);
            services.AddSingleton(new HttpClient());
            services.AddDbContext<FFXIVVenuesDbContext>();
            services.AddSingleton<IMapFactory>(new MapFactory(_configuration));
            services.AddSingleton<IAuthorizationManager, AuthorizationManager>();
            services.AddSingleton<IChangeBroker, ChangeBroker>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IEnumerable<AuthorizationKey>>(authorizationKeys);
            services.AddHttpClient();
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FFXIV Venues API", Version = "v1" }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            if (_configuration.GetValue("HttpsOnly", true))
                app.UseHttpsRedirection();

            if (_configuration.GetValue("EnableSwagger", true))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FFXIV Venues API v1"));
            }

            app.UseCors(pb => pb
                    .SetIsOriginAllowed(s => true)
                    .AllowCredentials()
                    .AllowAnyHeader())
                .UseCookiePolicy(new CookiePolicyOptions()
                {
                    MinimumSameSitePolicy = SameSiteMode.Lax
                })
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseWebSockets()
                .UseEndpoints(endpoints => endpoints.MapControllers());

        }
    }
}
