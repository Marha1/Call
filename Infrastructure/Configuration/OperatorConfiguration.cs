using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class OperatorConfiguration : IEntityTypeConfiguration<Operator>
{
    public void Configure(EntityTypeBuilder<Operator> builder)
    {
        builder.ToTable("Operators");

        builder.Property(o => o.AssignedDepartment)
            .HasConversion<string>()
            .IsRequired();

        builder.HasMany(o => o.AssignedRequests)
            .WithOne(r => r.Operator)
            .HasForeignKey(r => r.OperatorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}