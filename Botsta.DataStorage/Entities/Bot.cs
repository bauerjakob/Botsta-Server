using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Botsta.DataStorage.Entities
{
    public class Bot
    {
        [Key]
        [ForeignKey(nameof(ChatPracticant))]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }

        public User Owner { get; set; }

        public virtual ChatPracticant ChatPracticant { get; set; }
    }
}
