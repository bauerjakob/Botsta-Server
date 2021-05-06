using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;

namespace Botsta.Server.Middelware
{
    public interface ISessionController
    {
        public ClaimsPrincipal GetClaims();
        public User GetUser();
        public User GetUser(string token);
        public Task<ChatPracticant> GetPracticantFromToken(string token);
    }
}
