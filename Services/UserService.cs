using Microsoft.EntityFrameworkCore;
using UserManager.Data;
using UserManager.Models;

namespace UserManager.Services;

/// <summary>
/// EF Core implementation of <see cref="IUserService"/>.
///
/// Uses <see cref="IDbContextFactory{TContext}"/> rather than a single injected
/// DbContext. In Blazor Server a component can trigger overlapping async work on
/// one circuit, and a DbContext is not thread-safe; creating a short-lived context
/// per operation avoids "a second operation started on this context" errors.
/// </summary>
public class UserService : IUserService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public UserService(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<User>> GetAllAsync(string? search = null)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var query = db.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(u =>
                EF.Functions.Like(u.FirstName, $"%{term}%") ||
                EF.Functions.Like(u.LastName, $"%{term}%") ||
                EF.Functions.Like(u.Email, $"%{term}%"));
        }

        return await query
            .OrderByDescending(u => u.CreatedAt)
            .ThenBy(u => u.LastName)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UserSaveResult> CreateAsync(User user)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        if (await EmailTakenAsync(db, user.Email, excludeId: null))
        {
            return new UserSaveResult(false, "Użytkownik z tym adresem e-mail już istnieje.");
        }

        Normalize(user);
        user.Id = 0;
        user.CreatedAt = DateTime.UtcNow;

        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync();
            return new UserSaveResult(true, User: user);
        }
        catch (DbUpdateException)
        {
            return new UserSaveResult(false, "Nie udało się zapisać — adres e-mail jest już zajęty.");
        }
    }

    public async Task<UserSaveResult> UpdateAsync(User user)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var existing = await db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existing is null)
        {
            return new UserSaveResult(false, "Nie znaleziono użytkownika do edycji.");
        }

        if (await EmailTakenAsync(db, user.Email, excludeId: user.Id))
        {
            return new UserSaveResult(false, "Inny użytkownik już używa tego adresu e-mail.");
        }

        Normalize(user);

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        existing.Phone = user.Phone;
        existing.Department = user.Department;
        existing.IsActive = user.IsActive;

        try
        {
            await db.SaveChangesAsync();
            return new UserSaveResult(true, User: existing);
        }
        catch (DbUpdateException)
        {
            return new UserSaveResult(false, "Nie udało się zapisać — adres e-mail jest już zajęty.");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            return false;
        }

        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<UserStats> GetStatsAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var total = await db.Users.CountAsync();
        var active = await db.Users.CountAsync(u => u.IsActive);
        return new UserStats(total, active, total - active);
    }

    private static async Task<bool> EmailTakenAsync(AppDbContext db, string email, int? excludeId)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await db.Users.AnyAsync(u =>
            u.Email.ToLower() == normalized && (excludeId == null || u.Id != excludeId));
    }

    private static void Normalize(User user)
    {
        user.FirstName = user.FirstName.Trim();
        user.LastName = user.LastName.Trim();
        user.Email = user.Email.Trim();
        user.Phone = string.IsNullOrWhiteSpace(user.Phone) ? null : user.Phone.Trim();
    }
}
