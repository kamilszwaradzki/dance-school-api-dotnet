using DanceSchool.Api.Domain.Enums;

namespace DanceSchool.Api.Domain.Entities;

public class User : Entity
{
    public string EmailAddress { get; set; } = null!;
    public string? Name  { get; set; }
    public UserRole Role { get; set; }
    public string PasswordHash { get; set; } = null!;
    public ICollection<DanceGroup> InstructedGroups { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Pass> Passes { get; set; } = [];
    public ICollection<Attendance> Attendances { get; set; } = [];
}
