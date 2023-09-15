using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FFXIVVenues.Api.Security.UserAuthentication;

public interface IUserService
{
    User GetCurrentUser();
}

public class UserService : IUserService
{
    private readonly HttpContext _httpContext;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContext = httpContextAccessor.HttpContext;
    }

    public User GetCurrentUser()
    {
        if (this._httpContext.User.Identity?.IsAuthenticated != true)
            return null;
        
        var user = new User();
        string avatar = null;
        string banner = null;
        foreach (var claim in this._httpContext.User.Claims)
        {
            switch (claim.Type)
            {
                case ClaimTypes.Sid:
                    user.Id = claim.Value;
                    break;
                case ClaimTypes.Name:
                    user.DisplayName = claim.Value;
                    break;
                case ClaimTypes.NameIdentifier:
                    user.Username = claim.Value;
                    break;
                case DiscordClaimTypes.Avatar:
                    avatar = claim.Value;
                    break;
                case DiscordClaimTypes.Banner:
                    banner = claim.Value;
                    break;
            }
        }

        if (avatar != null)
            user.AvatarUri = $"https://cdn.discordapp.com/avatars/{user.Id}/{avatar}.jpg";
        if (banner != null)
            user.BannerUri = $"https://cdn.discordapp.com/banners/{user.Id}/{banner}.jpg";

        return user;
    }
    
}

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string AvatarUri { get; set; }
    public string BannerUri { get; set; }
}