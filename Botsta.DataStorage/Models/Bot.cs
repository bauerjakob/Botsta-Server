using System;
using System.ComponentModel.DataAnnotations;

namespace Botsta.DataStorage.Models
{
    public class Bot
    {
        [Key]
        public Guid BotId { get; set; }

        [Required]
        public string BotName { get; set; }

        public string WebhookUrl { get; set; }

        public string HashedApiKey { get; set; }
    }
}
