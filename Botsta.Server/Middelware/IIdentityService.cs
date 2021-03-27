using System;
using System.Threading.Tasks;
using Botsta.DataStorage.Models;

namespace Botsta.Server.Middelware
{
    public interface IIdentityService
    {
        public Task<User> RegisterUserAsync(string username, string password);
        public Task<(string apiKey, Bot bot)> RegisterBotAsync(string botName, User owner, string webhookUrl = null);
        public string LoginUser(string username, string password);
        public string LoginBot(string botName, string apiKey);
    }
}
