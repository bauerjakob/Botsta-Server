using System;
using Botsta.DataStorage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botsta.DataStorage
{
    public class BotstaDbContext : DbContext
    {

        public BotstaDbContext(DbContextOptions<BotstaDbContext> options) : base(options)
        {
        }

        public DbSet<Chatroom> Chatrooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Bot> Bots { get; set; }
    }
}
