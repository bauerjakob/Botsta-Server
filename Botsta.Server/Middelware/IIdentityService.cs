using System;
using System.Threading.Tasks;
using Botsta.DataStorage.Models;

namespace Botsta.Server.Middelware
{
    public interface IIdentityService
    {
        public Task<User> RegisterAsync(string username, string password);
        public string Login(string username, string password);
    }
}
