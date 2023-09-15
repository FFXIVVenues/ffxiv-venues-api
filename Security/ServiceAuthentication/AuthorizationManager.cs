using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace FFXIVVenues.Api.Security.ServiceAuthentication
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
            if (!authorizationHeader.Any())
                return null;

            
            var parsed = AuthenticationHeaderValue.TryParse(authorizationHeader.First(), out var authenticationHeader);

            if (parsed && authenticationHeader != null)
                return authenticationHeader.Parameter;

            return null;
        }

        public AuthorizationKey GetKey(string key = null)
        {
            if (key == null)
                key = this.GetKeyString();
            return _authorizationkeys.FirstOrDefault(k => k.Key == key);
        }

        public IAuthorizationCheck Check(string key = null)
        {
            var authKey = this.GetKey(key);
            if (authKey == null)
                return new NonAuthorizationCheck();
            return new AuthorizationCheck(authKey);
        }

    }

}
