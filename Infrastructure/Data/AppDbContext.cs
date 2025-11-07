using Microsoft.EntityFrameworkCore;
using TipJar.Domain.Entities;

namespace TipJar.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Tip> Tips { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasMany(u => u.Tips)
                .WithOne()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Metadata.FindNavigation(nameof(User.Tips))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            entity.Property(u => u.Username)
                .HasField("_username")
                .HasColumnName("Username")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            entity.Property(u => u.PasswordHash)
                .HasField("_passwordHash")
                .HasColumnName("PasswordHash")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            entity.Property(u => u.GrossTips)
                .HasField("_grossTips")
                .HasColumnName("GrossTips")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Tip>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(t => t.Amount)
                .HasPrecision(18, 2);
        });
    }
}