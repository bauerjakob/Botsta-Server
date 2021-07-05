using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Botsta.Core.Extentions
{
    public static class ClaimsExtensions
    {
        public static string GetSubject(this IEnumerable<Claim> claims)
        {
            return claims
                .Single(c => c.Properties.Any() ? c.Properties.FirstOrDefault().Value == JwtRegisteredClaimNames.Sub : false)?.Value;
        }

        public static Guid GetSessionId(this IEnumerable<Claim> claims)
        {
            var sessionId = claims
                .Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            return Guid.Parse(sessionId);
        }
    }
}
