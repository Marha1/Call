using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class UserRequestConfiguration : IEntityTypeConfiguration<UserRequest>
{
    public void Configure(EntityTypeBuilder<UserRequest> builder)
    {
        builder.ToTable("UserRequests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Topic)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.Status)
            .HasConversion<int>();

        builder.HasOne(r => r.User)
            .WithMany(u => u.Requests)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с оператором
        builder.HasOne(r => r.Operator)
            .WithMany(o => o.AssignedRequests)
            .HasForeignKey(r => r.OperatorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}