using UserManager.Models;

namespace UserManager.Services;

/// <summary>Outcome of a create/update operation, carrying a friendly error if it failed.</summary>
public record UserSaveResult(bool Success, string? Error = null, User? User = null);

/// <summary>Aggregate counts shown on the dashboard.</summary>
public record UserStats(int Total, int Active, int Inactive);

/// <summary>
/// CRUD contract for <see cref="User"/> records. Keeping data access behind this
/// interface lets the UI stay free of EF Core concerns and keeps the boundary testable.
/// </summary>
public interface IUserService
{
    Task<List<User>> GetAllAsync(string? search = null);
    Task<User?> GetByIdAsync(int id);
    Task<UserSaveResult> CreateAsync(User user);
    Task<UserSaveResult> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<UserStats> GetStatsAsync();
}
