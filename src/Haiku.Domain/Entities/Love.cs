namespace Haiku.Domain.Entities;

public class Love
{
    public virtual Guid Id { get; set; }
    public virtual Poem Poem { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual DateTime CreatedAt { get; set; }
}
