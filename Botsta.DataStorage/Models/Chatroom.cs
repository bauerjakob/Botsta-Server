using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Botsta.DataStorage.Models
{
    [Table("Chatroom")]
    public class Chatroom
    {
        [Key]
        public Guid ChatroomId { get; set; }

        public IEnumerable<User> Users { get; set; }

        public IEnumerable<Bot> Bots { get; set; }

        public IEnumerable<Message> Messages { get; set; }
    }
}
