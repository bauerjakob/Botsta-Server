using System;
using Botsta.DataStorage.Models;

namespace Botsta.Server.Middelware
{
    public interface ISessionController
    {
        public User GetUser();
    }
}
