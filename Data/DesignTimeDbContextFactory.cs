using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserManager.Data;

/// <summary>
/// Used by the EF Core CLI (migrations, database update) at design time.
/// Providing this keeps tooling from booting the full web host — and therefore
/// from running the startup migrate/seed in <see cref="DbInitializer"/>.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=usermanager.db")
            .Options;

        return new AppDbContext(options);
    }
}
