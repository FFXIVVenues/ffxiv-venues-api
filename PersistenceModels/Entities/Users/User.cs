using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FFXIVVenues.Api.PersistenceModels.Entities.Users;

[Table("Users", Schema = nameof(Entities.Users))]
public class User
{
    [Key] public string Id { get; init; }
    public string DiscordAccessToken { get; set; }
    public string DiscordRefreshToken { get; set; }
    public DateTimeOffset DiscordTokenExpiry { get; set; }
    public string[] DiscordScopes { get; set; }
}