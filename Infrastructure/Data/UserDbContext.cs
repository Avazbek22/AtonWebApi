using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация сущности User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Guid);
            entity.HasIndex(u => u.Login).IsUnique();

            entity.Property(u => u.Login)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(u => u.CreatedOn)
                .IsRequired();

            entity.Property(u => u.CreatedBy)
                .IsRequired()
                .HasMaxLength(50);

            // ModifiedOn, ModifiedBy, RevokedOn, RevokedBy — nullable
            entity.Property(u => u.ModifiedBy)
                .HasMaxLength(50);

            entity.Property(u => u.RevokedBy)
                .HasMaxLength(50);
        });
    }
}