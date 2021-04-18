using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Botsta.DataStorage.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        [ForeignKey(nameof(ChatPracticant))]
        public Guid Id { get; set; }

        public ChatPracticant ChatPracticant { get; set; }

        public virtual IEnumerable<Bot> Bots { get; set; } = new List<Bot>();
    }
}
