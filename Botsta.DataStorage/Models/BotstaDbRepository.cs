using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botsta.DataStorage.Models
{
    public class BotstaDbRepository : IBotstaDbRepository
    {
        private BotstaDbContext _dbContext;

        public BotstaDbRepository(BotstaDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public IEnumerable<Message> GetMessages(string chatroomId)
        {
            return _dbContext.Chatrooms?
                .Single(c => c.ChatroomId.ToString() == chatroomId).Messages;
        }

        public User GetUserByUsername(string username)
        {
            return _dbContext.Users?.Single(u => u.Username == username);
        }

        public User GetUserById(string userId)
        {
            return _dbContext.Users?.Single(u => u.UserId.ToString() == userId);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _dbContext.Users.ToList();
        }
    }
}
