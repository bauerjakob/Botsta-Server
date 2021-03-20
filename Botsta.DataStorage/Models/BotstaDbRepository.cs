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

        public IEnumerable<Message> GetMessages()
        {
            return _dbContext.Messages?.ToList();
        }

        public User GetUserByUsername(string username)
        {
            return _dbContext.Users?.Single(u => u.Username == username);
        }
    }
}
