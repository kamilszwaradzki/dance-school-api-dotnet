using DanceSchool.Api.Domain.Enums;

namespace DanceSchool.Api.Domain.Entities;

public class Enrollment : Entity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int DanceGroupId { get; set; }
    public DanceGroup DanceGroup { get; set; } = null!;
    public EnrollmentStatus Status { get; set; }
    public int? Position { get; set; }
}
