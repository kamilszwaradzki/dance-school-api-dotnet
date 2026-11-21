using DanceSchool.Api.Domain.Enums;

namespace DanceSchool.Api.Domain.Entities;

public class User : Entity
{
    public string EmailAddress
    {
        get;
        set => field = NormalizeEmail(value);
    } = null!;

    public string? Name { get; set; }
    public UserRole Role { get; set; }
    public string PasswordHash { get; set; } = null!;
    public ICollection<DanceGroup> InstructedGroups { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Pass> Passes { get; set; } = [];
    public ICollection<Attendance> Attendances { get; set; } = [];

    /// <summary>
    /// Single source of the e-mail rule (Rails: <c>normalizes :email_address</c>).
    /// Use it also when looking users up by e-mail, e.g. at login.
    /// </summary>
    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
