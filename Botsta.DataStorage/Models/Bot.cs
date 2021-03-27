using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Botsta.DataStorage.Models
{
    public class Bot
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string BotName { get; set; }

        public string WebhookUrl { get; set; }

        [Required]
        public string ApiKeyHash { get; set; }

        [Required]
        public string ApiKeySalt { get; set; }

        [Required]
        public DateTimeOffset Registerd { get; set; }

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }

        public User Owner { get; set; }

        IEnumerable<Chatroom> Chatrooms;
    }
}
