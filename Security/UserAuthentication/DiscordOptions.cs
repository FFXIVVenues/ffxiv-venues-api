using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace FFXIVVenues.Api.Security.UserAuthentication;

public class DiscordOptions : OAuthOptions
{
    public DiscordOptions()
    {
        AuthorizationEndpoint = DiscordDefaults.AuthorizationEndpoint + "?prompt=none";
        TokenEndpoint = DiscordDefaults.TokenEndpoint;
        CallbackPath = new PathString("/signin-discord");
        UserInformationEndpoint = DiscordDefaults.UserInformationEndpoint;
        Scope.Add("identify");

        ClaimActions.MapJsonKey(ClaimTypes.Sid, "id", ClaimValueTypes.UInteger64);
        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "username", ClaimValueTypes.String);
        ClaimActions.MapJsonKey(ClaimTypes.Name, "global_name", ClaimValueTypes.String);
        ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);
        ClaimActions.MapJsonKey("urn:discord:avatar", "avatar", ClaimValueTypes.String);
        ClaimActions.MapJsonKey("urn:discord:verified", "verified", ClaimValueTypes.Boolean);
    }
        
}