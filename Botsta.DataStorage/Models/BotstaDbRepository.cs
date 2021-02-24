using System;
using System.Collections.Generic;
using System.Linq;

namespace Botsta.DataStorage.Models
{
    public class BotstaDbRepository : IBotstaDbRepository
    {
        private BotstaDbContext _dbContext;

        public BotstaDbRepository(BotstaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Message> GetMessages()
        {
            return _dbContext.Messages?.ToList();
        }
    }
}
