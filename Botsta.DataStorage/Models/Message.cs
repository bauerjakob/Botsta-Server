using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Botsta.DataStorage.Models
{
    [Table("Message")]
    public class Message
    {
        private Guid messageId;

        [Key]
        public Guid MessageId { get => messageId; set => messageId = value; }

        [Required]
        public string MessageJson { get; set; }

        public string MyProperty { get; set; }
    }
}
