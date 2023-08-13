using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using FFXIVVenues.Api.PersistenceModels.Entities.Venues;

namespace FFXIVVenues.Api.Security
{
    public class AuthorizationManager : IAuthorizationManager
    {

        private readonly IEnumerable<AuthorizationKey> _authorizationkeys;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationManager(IEnumerable<AuthorizationKey> authorizationkeys, IHttpContextAccessor httpContextAccessor)
        {
            _authorizationkeys = authorizationkeys;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated() => this.GetKeyString() != null;
        
        public string GetKeyString()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"] ?? string.Empty;
            Console.WriteLine($"Authorization header is {authorizationHeader}");
            if (!authorizationHeader.Any())
                return null;

            
            var parsed = AuthenticationHeaderValue.TryParse(authorizationHeader.First(), out var authenticationHeader);

            Console.WriteLine($"Authentication header value is {authenticationHeader}");
            if (parsed && authenticationHeader != null)
                return authenticationHeader.Parameter;

            return null;
        }

        public AuthorizationKey GetKey(string key = null)
        {
            if (key == null)
            {
                Console.WriteLine($"Key given is null; fetching key from header;");
                key = this.GetKeyString();
            }
            Console.WriteLine($"Searching for auth key for {key}");
            return _authorizationkeys.FirstOrDefault(k => k.Key == key);
        }

        public IAuthorizationCheck Check(string key = null)
        {
            Console.WriteLine($"Check requested for key {key}");
            var authKey = this.GetKey(key);
            if (authKey == null)
            {
                Console.WriteLine($"Null auth key; return NonAuthCheck object");
                return new NonAuthorizationCheck();
            }

            Console.WriteLine($"Auth key found; returning {JsonSerializer.Serialize(authKey)}");
            return new AuthorizationCheck(authKey);
        }

    }

}
