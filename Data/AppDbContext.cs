using Microsoft.EntityFrameworkCore;
using UserManager.Models;

namespace UserManager.Data;

/// <summary>
/// Application database context. Backed by SQLite so the project runs anywhere
/// with no external database server to install.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Department)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
