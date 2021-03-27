using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Botsta.DataStorage.Models
{
    [Table("Message")]
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string MessageJson { get; set; }

        [ForeignKey(nameof(Chatrooms))]
        public Guid ChatroomId { get; set; }

        [Required]
        public SenderType SenderType { get; set; }

        [Required]
        public Guid SenderId { get; set; }

        public IEnumerable<Chatroom> Chatrooms { get; set; }
    }
}
