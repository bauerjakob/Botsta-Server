using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Botsta.DataStorage.Models
{
    public interface IBotstaDbRepository
    {
        public IEnumerable<Message> GetMessages();

        public User GetUserByUsername(string username);

        public Task AddUserToDb(User user);
    }
}
