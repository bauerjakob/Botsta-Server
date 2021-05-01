using System;
using Botsta.DataStorage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Botsta.DataStorage.Mappings
{
    public class ChatroomMapping : IEntityTypeConfiguration<Chatroom>
    {
        public void Configure(EntityTypeBuilder<Chatroom> builder)
        {
            builder.Property(p => p.Type).HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ChatroomType>(v));
        }
    }
}
