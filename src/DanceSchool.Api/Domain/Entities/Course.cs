using DanceSchool.Api.Domain.Enums;

namespace DanceSchool.Api.Domain.Entities;

public class Course : Entity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CourseLevel Level { get; set; }
    public ICollection<DanceGroup> Groups { get; set; } = [];
}
