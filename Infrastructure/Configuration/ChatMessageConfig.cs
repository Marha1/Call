using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class ChatMessageConfig : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.Property(x => x.Message).IsRequired().HasMaxLength(100);
        builder.Property(x => x.TicketId).IsRequired();
        builder.Property(x => x.SenderId).IsRequired();
        builder.HasKey(x => x.id);
    }
}