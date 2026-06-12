namespace Haiku.Domain.Entities;

public class Bookmark
{
    public virtual Guid Id { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual Poem Poem { get; set; } = null!;
    public virtual DateTime CreatedAt { get; set; }
}
