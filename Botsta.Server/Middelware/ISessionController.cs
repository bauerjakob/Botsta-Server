using System;
using Botsta.DataStorage.Entities;

namespace Botsta.Server.Middelware
{
    public interface ISessionController
    {
        public User GetUser();
        public User GetUser(string token);
    }
}
