using System;
using Botsta.DataStorage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Botsta.DataStorage.Mappings
{
    public class ChatPracticantMapping : IEntityTypeConfiguration<ChatPracticant>
    {
        public void Configure(EntityTypeBuilder<ChatPracticant> builder)
        {
            builder.HasIndex(p => p.Name).IsUnique();
            builder.Property(p => p.Type).HasConversion(
                t => t.ToString(),
                t => Enum.Parse<PracticantType>(t)
                );
        }
    }
}
