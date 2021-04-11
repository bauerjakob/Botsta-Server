using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Botsta.DataStorage.Entities
{
    [Table("ChatPracticant")]
    public class ChatPracticant
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTimeOffset Registerd { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SecretHash { get; set; }

        [Required]
        public string SecretSalt { get; set; }

        [Required]
        public PracticantType Type { get; set; }

        public virtual IEnumerable<Chatroom> Chatrooms { get; set; } = new List<Chatroom>();
    }
}
