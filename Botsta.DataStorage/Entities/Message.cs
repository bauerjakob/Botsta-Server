using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Botsta.DataStorage.Entities
{
    [Table("Message")]
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column("Message")]
        public string Msg { get; set; }

        [ForeignKey(nameof(Chatroom))]
        public Guid ChatroomId { get; set; }

        [Required]
        [ForeignKey(nameof(Sender))]
        public Guid SenderId { get; set; }

        [Required]
        public DateTimeOffset SendTime { get; set; }

        public ChatPracticant Sender { get; set; }
        public Chatroom Chatroom { get; set; }
    }
}
