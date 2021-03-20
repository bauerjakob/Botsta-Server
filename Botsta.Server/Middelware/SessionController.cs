using System;
using Botsta.DataStorage.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace Botsta.Server.Middelware
{
    public class SessionController : ISessionController
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IBotstaDbRepository _dbContext;

        public SessionController(IHttpContextAccessor httpContext, IBotstaDbRepository dbContext)
        {
            _httpContext = httpContext;
            _dbContext = dbContext;
        }

        public User GetUser()
        {
            var userId = _httpContext?.HttpContext?.User?.Claims
                .SingleOrDefault(c => c.Properties.Any() ? c.Properties.FirstOrDefault().Value == JwtRegisteredClaimNames.Sub : false)?.Value;

            return _dbContext.GetUserById(userId);
        }
    }
}
