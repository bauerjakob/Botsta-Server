using System;
using Botsta.DataStorage.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Botsta.DataStorage;
using Botsta.Core.Extentions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Botsta.Core.Services
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
            var userId = _httpContext?.HttpContext?.User?.Claims.GetSubject();
            return _dbContext.GetUserById(userId);
        }

        public ClaimsPrincipal GetClaims()
        {
            return _httpContext?.HttpContext?.User;
        }

        public User GetUser(string token)
        {
            var principal = _identityService.ValidateToken(token);
            var userId = principal.Claims.GetSubject();

            return _dbContext.GetUserById(userId);
        }

        public async Task<ChatPracticant> GetChatPracticantAsync()
        {
            var practicantId = _httpContext?.HttpContext?.User?.Claims.GetSubject();
            return await _dbContext.GetChatPracticantAsync(Guid.Parse(practicantId));
        }

        public async Task<ChatPracticant> GetChatPracticantFromToken(string token)
        {
            var principal = _identityService.ValidateToken(token);
            var practicantId = principal.Claims.GetSubject();

            return await _dbContext.GetChatPracticantAsync(Guid.Parse(practicantId));
        }

        public Guid GetSessionId()
        {
            return _httpContext.HttpContext.User.Claims.GetSessionId();
        }

        public Guid GetSessionId(string token)
        {
            var principal = _identityService.ValidateToken(token);

            return principal.Claims.GetSessionId();
        }
    }
}
