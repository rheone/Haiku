namespace Haiku.Domain.Entities;

public class Vote
{
    public virtual Guid Id { get; set; }
    public virtual Poem Poem { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual sbyte Value { get; set; }
    public virtual DateTime CreatedAt { get; set; }
}
