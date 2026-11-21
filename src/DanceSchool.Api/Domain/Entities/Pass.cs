namespace DanceSchool.Api.Domain.Entities;

public class Pass : Entity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int EntriesTotal { get; set; }
    public int EntriesLeft { get; set; }
    public DateTime ValidUntil { get; set; }

    public bool IsUsable(DateTime now) => ValidUntil > now && EntriesLeft > 0;

    public bool IsExpiringSoon(DateTime now)
    {
        return ValidUntil >= now && now.AddDays(7) >= ValidUntil;
    }
}
