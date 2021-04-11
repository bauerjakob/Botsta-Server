using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;

namespace Botsta.Server.Middelware
{
    public interface IIdentityService
    {
        public Task<User> RegisterUserAsync(string username, string password);
        public Task<(string apiKey, Bot bot)> RegisterBotAsync(string botName, User owner, string webhookUrl = null);
        public Task<string> LoginAsync(string name, string secret);
        public ClaimsPrincipal ValidateToken(string token);
    }
}
