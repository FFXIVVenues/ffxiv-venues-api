using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

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

        public string GetKey()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"] ?? Microsoft.Extensions.Primitives.StringValues.Empty;
            if (!authorizationHeader.Any())
                return null;

            var parsed = AuthenticationHeaderValue.TryParse(authorizationHeader.First(), out var authenticationHeader);

            if (parsed && authenticationHeader != null)
                return authenticationHeader.Parameter;

            return null;
        }

        public IAuthorizationCheck Check(string key = null)
        {
            if (key == null)
            {
                key = GetKey();

                if (key == null)
                    return new NonAuthorizationCheck();
            }

            var authKey = _authorizationkeys.FirstOrDefault(k => k.Key == key);
            if (authKey == null) return new NonAuthorizationCheck();
            return new AuthorizationCheck(authKey);
        }

    }

}
