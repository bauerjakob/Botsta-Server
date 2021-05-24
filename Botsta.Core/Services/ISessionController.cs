using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;

namespace Botsta.Core.Services
{
    public interface ISessionController
    {
        public ClaimsPrincipal GetClaims();
        public User GetUser();
        public User GetUser(string token);

        public Task<ChatPracticant> GetChatPracticantAsync();
        public Task<ChatPracticant> GetChatPracticantFromToken(string token);
    }
}
