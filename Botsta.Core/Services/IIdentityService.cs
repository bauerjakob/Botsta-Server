using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;
using Botsta.Core.Dto;

namespace Botsta.Core.Services
{
    public interface IIdentityService
    {
        public Task<User> RegisterUserAsync(string username, string password);
        public Task<(string apiKey, Bot bot)> RegisterBotAsync(string botName, User owner, bool isPublic);
        public Task<LoginResponse> LoginAsync(string name, string secret);
        public ClaimsPrincipal ValidateToken(string token);
        Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken);
        Task<RefreshTokenResponse> RefreshTokenAsync(ClaimsPrincipal claims);
    }
}
