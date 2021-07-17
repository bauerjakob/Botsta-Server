using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;

namespace Botsta.DataStorage
{
    public interface IBotstaDbRepository
    {
        public Task<Chatroom> GetChatroomByIdAsync(Guid id);

        public Task<ChatPracticant> GetChatPracticantAsync(Guid id);
        public IEnumerable<ChatPracticant> GetChatPracticants(IEnumerable<Guid> ids);
        public Task<ChatPracticant> GetChatPracticantAsync(string name);
        public IEnumerable<Bot> GetBots(Guid owner);

        public IEnumerable<Message> GetMessages(string chatroomId);

        public IEnumerable<User> GetAllUsers();
        public IEnumerable<ChatPracticant> GetAllChatPracticants();

        public User GetUserByUsername(string username);

        public User GetUserById(string userId);

        public Task DeleteMessagesAsync(IEnumerable<Guid> messageIds, Guid sessionId);

        public Task AddKeyExchangeToDbAsync(Guid chatPracticantId, Guid sessionId, string publicKey);

        public Bot GetBotById(string botId);

        public Task<User> AddUserToDbAsync(string username, string passwordHash, string passwordSalt);

        public Task AddChatroomToDbAsync(Chatroom chatroom);

        public Task AddMessageToDb(Message message);

        public Bot GetBotByName(string botName);

        public Task<Bot> AddBotToDbAsync(User owner, string botName, bool isPublic, string apiKeyHash, string apiKeySalt);
    }
}
