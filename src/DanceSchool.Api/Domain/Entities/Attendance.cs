namespace DanceSchool.Api.Domain.Entities;

public class Attendance : Entity
{
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public bool? Present { get; set; }
    public int? PassId { get; set; }
    public Pass? Pass { get; set; }
}
