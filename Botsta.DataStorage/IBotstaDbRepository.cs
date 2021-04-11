﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;

namespace Botsta.DataStorage
{
    public interface IBotstaDbRepository
    {
        public IEnumerable<ChatPracticant> GetChatPracticants(IEnumerable<Guid> ids);
        public Task<ChatPracticant> GetChatPracticantAsync(string name);

        public IEnumerable<Message> GetMessages(string chatroomId);

        public IEnumerable<User> GetAllUsers();

        public User GetUserByUsername(string username);

        public User GetUserById(string userId);

        public Bot GetBotById(string botId);

        public Task<User> AddUserToDbAsync(string username, string passwordHash, string passwordSalt);

        public Task AddChatroomToDbAsync(Chatroom chatroom);

        public Task AddMessageToDb(Message message);

        public Bot GetBotByName(string botName);

        public Task<Bot> AddBotToDbAsync(User owner, string botName, string apiKeyHash, string apiKeySalt);
    }
}