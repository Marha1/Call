using Domain.Models;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationContext : IdentityDbContext<AppUser>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Operator> Operators { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<UserRequest> UserRequests { get; set; }
    public DbSet<AttachmentUser> Attachments { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Регистрация конфигураций
        modelBuilder.ApplyConfiguration(new AppUserConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OperatorConfiguration());
        modelBuilder.ApplyConfiguration(new UserRequestConfiguration());
        modelBuilder.ApplyConfiguration(new AttachmentUserConfiguration());
    }
}