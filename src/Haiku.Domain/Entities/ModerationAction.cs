namespace Haiku.Domain.Entities;

public class ModerationAction
{
    public virtual Guid Id { get; set; }
    public virtual string ActionType { get; set; } = string.Empty;
    public virtual string TargetType { get; set; } = string.Empty;
    public virtual Guid TargetId { get; set; }
    public virtual User ActionedBy { get; set; } = null!;
    public virtual string Reason { get; set; } = string.Empty;
    public virtual DateTime CreatedAt { get; set; }
}
