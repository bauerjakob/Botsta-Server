using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Botsta.DataStorage.Models
{
    public class Bot
    {
        [Key]
        public Guid BotId { get; set; }

        [Required]
        public string BotName { get; set; }

        public string WebhookUrl { get; set; }

        [Required]
        public string HashedApiKey { get; set; }

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }

        public User Owner { get; set; }
    }
}
