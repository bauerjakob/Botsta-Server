using System;
using Botsta.DataStorage.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Botsta.DataStorage;

namespace Botsta.Server.Middelware
{
    public class SessionController : ISessionController
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IBotstaDbRepository _dbContext;
        private readonly IIdentityService _identityService;

        public SessionController(IHttpContextAccessor httpContext, IBotstaDbRepository dbContext, IIdentityService identityService)
        {
            _httpContext = httpContext;
            _dbContext = dbContext;
            _identityService = identityService;
        }

        public User GetUser()
        {
            var userId = _httpContext?.HttpContext?.User?.Claims
                .Single(c => c.Properties.Any() ? c.Properties.FirstOrDefault().Value == JwtRegisteredClaimNames.Sub : false)?.Value;

            return _dbContext.GetUserById(userId);
        }

        public User GetUser(string token)
        {
            var principal = _identityService.ValidateToken(token);
            var userId = principal.Claims
                .Single(c => c.Properties.Any() ? c.Properties.FirstOrDefault().Value == JwtRegisteredClaimNames.Sub : false)?.Value;

            return _dbContext.GetUserById(userId);
        }
    }
}
