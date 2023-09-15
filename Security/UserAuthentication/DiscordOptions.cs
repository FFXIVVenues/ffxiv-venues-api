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
        ClaimActions.MapJsonKey(DiscordClaimTypes.Avatar, "avatar", ClaimValueTypes.String);
        ClaimActions.MapJsonKey(DiscordClaimTypes.Banner, "banner", ClaimValueTypes.String);
        ClaimActions.MapJsonKey(DiscordClaimTypes.Verified, "verified", ClaimValueTypes.Boolean);
    }
        
}

public class DiscordClaimTypes
{
    public const string Avatar = "urn:discord:avatar";
    public const string Banner = "urn:discord:banner";
    public const string Verified = "urn:discord:verified";
}
