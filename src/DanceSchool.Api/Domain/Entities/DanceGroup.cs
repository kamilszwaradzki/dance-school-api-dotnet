namespace DanceSchool.Api.Domain.Entities;

public class DanceGroup : Entity
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public int InstructorId { get; set; }
    public User Instructor { get; set; } = null!;
    public DayOfWeek Weekday { get; set; }
    public TimeOnly StartsAt { get; set; }
    public int DurationMinutes { get; set; }
    public int Capacity { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Lesson> Lessons { get; set; } = [];
}
