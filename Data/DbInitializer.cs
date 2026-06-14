using Microsoft.EntityFrameworkCore;
using UserManager.Data;
using UserManager.Models;

namespace UserManager.Data;

/// <summary>
/// Applies pending migrations and seeds a few sample users on first run so the
/// app is not empty when opened for the first time. Runs once at startup.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var db = await factory.CreateDbContextAsync();

        await db.Database.MigrateAsync();

        if (await db.Users.AnyAsync())
        {
            return;
        }

        db.Users.AddRange(
            new User
            {
                FirstName = "Anna",
                LastName = "Kowalska",
                Email = "anna.kowalska@example.com",
                Phone = "+48 601 234 567",
                Department = Department.HR,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-12)
            },
            new User
            {
                FirstName = "Piotr",
                LastName = "Nowak",
                Email = "piotr.nowak@example.com",
                Phone = "+48 602 345 678",
                Department = Department.IT,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new User
            {
                FirstName = "Magdalena",
                LastName = "Wiśniewska",
                Email = "magdalena.wisniewska@example.com",
                Department = Department.Sales,
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            });

        await db.SaveChangesAsync();
    }
}
