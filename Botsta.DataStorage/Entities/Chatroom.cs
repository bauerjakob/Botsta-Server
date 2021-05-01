using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Botsta.DataStorage.Entities
{
    [Table("Chatroom")]
    public class Chatroom
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public ChatroomType Type;

        public string Name;

        public virtual IEnumerable<ChatPracticant> ChatPracticants { get; set; } = new List<ChatPracticant>();

        public virtual IEnumerable<Message> Messages { get; set; } = new List<Message>();
    }
}
