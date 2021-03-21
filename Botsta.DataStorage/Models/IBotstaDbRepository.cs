using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Botsta.DataStorage.Models
{
    public interface IBotstaDbRepository
    {
        public IEnumerable<Message> GetMessages(string chatroomId);

        public IEnumerable<User> GetAllUsers();

        public User GetUserByUsername(string username);

        public User GetUserById(string userId);

        public Bot GetBotById(string botId);

        public Task AddUserToDb(User user);

        public Task AddChatroomToDbAsync(Chatroom chatroom);

        public Task AddMessageToDb(Message message);
    }
}
