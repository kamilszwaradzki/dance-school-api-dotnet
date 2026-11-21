namespace DanceSchool.Api.Domain.Entities;

public class Lesson : Entity
{
    public int DanceGroupId { get; set; }
    public DanceGroup DanceGroup { get; set; } = null!;
    public DateTime StartsAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public bool IsCancelled => CancelledAt != null;
    public ICollection<Attendance> Attendances { get; set; } = [];
}
