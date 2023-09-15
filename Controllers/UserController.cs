using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using FFXIVVenues.Api.Helpers;
using FFXIVVenues.Api.PersistenceModels.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FFXIVVenues.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string[] _scopes;
    private readonly string _redirectUri;
    private readonly string _authorizationPrompt;

    public UserController(IConfiguration config, IHttpClientFactory httpClientFactory, FFXIVVenuesDbContext dbContext)
    {
        this._httpClientFactory = httpClientFactory;
        this._clientId = config.GetValue<string>("DiscordApi:ClientId");
        this._clientSecret = config.GetValue<string>("DiscordApi:ClientSecret");
        this._redirectUri = config.GetValue<string>("DiscordApi:RedirectUri");
        this._authorizationPrompt = config.GetValue<string>("DiscordApi:AuthorizationPrompt");
        this._scopes = config.GetSection("DiscordApi:Scopes").Get<string[]>();
    }
    
    [HttpGet("me")]
    [Authorize]
    public string Me()
    {
        
        return this.HttpContext.User.Identity?.Name;
        // var authUri = 
        //     "https://discord.com/oauth2/authorize?response_type=code&client_id={client_id}&" +
        //     "scope={scopes}&state={state}&redirect_uri={redirect_uri}&prompt={prompt}"
        //     .Replace("{client_id}", this._clientId)
        //     .Replace("{state}", IdHelper.GenerateId(24))
        //     .Replace("{redirect_uri}", this._redirectUri)
        //     .Replace("{scopes}", string.Join(" ", this._scopes))
        //     .Replace("{prompt}", this._authorizationPrompt);
        // return this.Redirect(authUri);
    }

    [HttpGet("code")]
    public async Task<ActionResult> Authorise([FromQuery] string code, [FromQuery] string state)
    {
        
        // todo Rejection handling
        // todo State validation
        // todo Error handling for any of these requests
        // todo Store it and get a ffxivvenues.api key
        // todo Figure out how to get this back to the frontend
        using var httpClient = this._httpClientFactory.CreateClient();
        var tokenResponse = await httpClient.PostAsync("https://discord.com/api/oauth2/token", new FormUrlEncodedContent(new []
        {
            new KeyValuePair<string, string>("client_id", this._clientId),
            new KeyValuePair<string, string>("client_secret", this._clientSecret),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", this._redirectUri),
        }));
        var tokenDetails = await tokenResponse.Content.ReadFromJsonAsync<AuthorizationTokenDto>();
        
        
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDetails.access_token);
        var meResponse = await httpClient.GetAsync("https://discord.com/api/users/@me");
        var meDetails = await meResponse.Content.ReadFromJsonAsync<DiscordUserDto>();
        Console.WriteLine($"User {meDetails.global_name} ({meDetails.username}) fetched their banner!");
        return this.Ok(meDetails);
        var avatar = await httpClient.GetAsync($"https://cdn.discordapp.com/avatars/{meDetails.id}/{meDetails.avatar}.jpg");
        return this.File(await avatar.Content.ReadAsStreamAsync(), avatar.Content.Headers.ContentType?.MediaType ?? "image/jpg");
    }
}

public class AuthorizationTokenDto
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string refresh_token { get; set; }
    public string scope { get; set; }
}

public class DiscordUserDto
{
    public string id { get; set; }
    public string username { get; set; }
    public string global_name { get; set; }
    public string discriminator { get; set; }
    public string avatar { get; set; }
    public bool verified { get; set; }
    public string email { get; set; }
    public int flags { get; set; }
    public string banner { get; set; }
    public int accent_color { get; set; }
    public int premium_type { get; set; }
    public int public_flags { get; set; }
}

