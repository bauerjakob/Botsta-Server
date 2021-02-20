using System;
using Microsoft.EntityFrameworkCore;

namespace Botsta.DataStorage.Models
{
    public class PsqlContext : DbContext
    {
        public PsqlContext(DbContextOptions<PsqlContext> options) : base(options)
        {
        }

        DbSet<Chatroom> Chatrooms { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Bot> Bots { get; set; }
    }
}
