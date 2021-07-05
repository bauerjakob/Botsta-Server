using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Botsta.DataStorage.Entities
{
    public class KeyExchange
    {
        [Key]
        public Guid SessionId { get; set; }

        public Guid ChatPracticantId { get; set; }

        public string PublicKey { get; set; }

        [ForeignKey(nameof(ChatPracticantId))]
        public ChatPracticant ChatPracticant { get; set; }
    }
}
