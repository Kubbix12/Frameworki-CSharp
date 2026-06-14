using System.ComponentModel.DataAnnotations;

namespace UserManager.Models;

/// <summary>
/// Organizational department a user belongs to.
/// Stored as an integer in the database (EF Core default for enums).
/// </summary>
public enum Department
{
    IT = 0,
    HR = 1,
    Sales = 2,
    Marketing = 3,
    Finance = 4,
    Operations = 5,
    Other = 6
}

/// <summary>
/// A managed user record. This is the single entity persisted to the database
/// and the model bound by the create/edit form.
/// </summary>
public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Imię musi mieć od 2 do 50 znaków.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwisko musi mieć od 2 do 50 znaków.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
    [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail.")]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Podaj poprawny numer telefonu.")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Wybierz dział.")]
    public Department Department { get; set; } = Department.Other;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}".Trim();
}

/// <summary>
/// Maps <see cref="Department"/> values to Polish UI labels so the enum names
/// (kept in English by convention) are presented in the user's language.
/// </summary>
public static class DepartmentLabels
{
    private static readonly IReadOnlyDictionary<Department, string> Map = new Dictionary<Department, string>
    {
        [Department.IT] = "IT",
        [Department.HR] = "Kadry (HR)",
        [Department.Sales] = "Sprzedaż",
        [Department.Marketing] = "Marketing",
        [Department.Finance] = "Finanse",
        [Department.Operations] = "Operacje",
        [Department.Other] = "Inny",
    };

    public static string ToPolish(this Department department) =>
        Map.TryGetValue(department, out var label) ? label : department.ToString();

    public static IEnumerable<Department> All => Map.Keys;
}
