namespace DanceSchool.Api.Domain;

public abstract class Entity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }   // UTC
    public DateTime UpdatedAt { get; set; }   // UTC
}
