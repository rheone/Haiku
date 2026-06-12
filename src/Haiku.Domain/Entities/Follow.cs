namespace Haiku.Domain.Entities;

public class Follow
{
    public virtual Guid Id { get; set; }
    public virtual User Follower { get; set; } = null!;
    public virtual User Followee { get; set; } = null!;
    public virtual DateTime CreatedAt { get; set; }
}
