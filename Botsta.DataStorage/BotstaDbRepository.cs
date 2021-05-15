using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Botsta.DataStorage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botsta.DataStorage
{
    public class BotstaDbRepository : IBotstaDbRepository
    {
        private BotstaDbContext _dbContext;

        public BotstaDbRepository(BotstaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChatPracticant> GetChatPracticantAsync(string name)
        {
            return await _dbContext
                .ChatPracticants
                .SingleAsync(p => p.Name == name);
        }

        public IEnumerable<ChatPracticant> GetChatPracticants(IEnumerable<Guid> ids)
        {
            return _dbContext.ChatPracticants.Where(
                    c => ids.Contains(c.Id)
                );
        }

        public async Task<ChatPracticant> GetChatPracticantAsync(Guid id)
        {
            return await _dbContext.ChatPracticants
                .Include(c => c.Chatrooms)
                .SingleAsync(
                    c => c.Id == id
                );
        }

        public async Task AddUserToDb(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _dbContext.Users.AddAsync(user);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> AddUserToDbAsync(string username, string passwordHash, string passwordSalt)
        {
            var userId = Guid.NewGuid();
            var practicant = new ChatPracticant
            {
                Id = userId,
                Name = username,
                SecretHash = passwordHash,
                SecretSalt = passwordSalt,
                Registerd = DateTimeOffset.UtcNow,
                Type = PracticantType.User
            };

            var user = new User
            {
                Id = userId
            };

            await _dbContext.ChatPracticants.AddAsync(practicant);
            await _dbContext.Users.AddAsync(user);

            await _dbContext.SaveChangesAsync();

            return user;
        }

        public IEnumerable<Message> GetMessages(string chatroomId)
        {
            return _dbContext.Chatrooms?
                .Single(c => c.Id.ToString() == chatroomId).Messages;
        }

        public User GetUserByUsername(string username)
        {
            return _dbContext.Users?
                .Include(u => u.ChatPracticant)
                    .ThenInclude(u => u.Chatrooms)
                    .ThenInclude(c => c.ChatPracticants)
                .Include(u => u.Bots)
                .Single(u => u.ChatPracticant.Name == username);
        }

        public User GetUserById(string userId)
        {
            return _dbContext.Users?
                .Include(u => u.ChatPracticant)
                    .ThenInclude(u => u.Chatrooms)
                        .ThenInclude(u => u.Messages)
                .Include(u => u.ChatPracticant)
                    .ThenInclude(u => u.Chatrooms)
                        .ThenInclude(u => u.ChatPracticants)
                .Include(u => u.Bots)
                .Single(u => u.Id.ToString() == userId);
        }


        public async Task<Chatroom> GetChatroomByIdAsync(Guid id)
        {
            return await _dbContext.Chatrooms?
                .Include(c => c.ChatPracticants)
                .Include(c => c.Messages)
                .SingleAsync(c => c.Id.Equals(id));
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _dbContext.Users
                .Include(u => u.ChatPracticant)
                    .ThenInclude(u => u.Chatrooms)
                .Include(u => u.Bots)
                .ToList();
        }

        public Bot GetBotById(string botId)
        {
            return _dbContext.Bots
                .Include(u => u.ChatPracticant)
                    .ThenInclude(u => u.Chatrooms)
                .Include(u => u.Owner)
                .Single(b => b.Id.ToString() == botId);
        }

        public async Task AddChatroomToDbAsync(Chatroom chatroom)
        {
            if (chatroom is null)
            {
                throw new ArgumentNullException(nameof(chatroom));
            }

            await _dbContext.Chatrooms.AddAsync(chatroom);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddMessageToDb(Message message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
        }

        public Bot GetBotByName(string botName)
        {
            return _dbContext.Bots
                .Include(u => u.ChatPracticant)
                    .ThenInclude(u => u.Chatrooms)
                .Include(u => u.Owner)
                .Single(b => b.ChatPracticant.Name == botName);
        }

        public async Task<Bot> AddBotToDbAsync(User owner, string botName, string apiKeyHash, string apiKeySalt)
        {
            var botId = Guid.NewGuid();
            var practicant = new ChatPracticant
            {
                Id = botId,
                Name = botName,
                SecretHash = apiKeyHash,
                SecretSalt = apiKeySalt,
                Registerd = DateTimeOffset.UtcNow,
                Type = PracticantType.Bot
            };

            var bot = new Bot()
            {
                Id = botId,
                Owner = owner
            };

            await _dbContext.ChatPracticants.AddAsync(practicant);
            await _dbContext.Bots.AddAsync(bot);
            await _dbContext.SaveChangesAsync();

            return bot;
        }
    }
}
