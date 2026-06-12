namespace Haiku.Domain.Entities;

public class HaikuTag
{
    public virtual Guid HaikuId { get; set; }
    public virtual int TagId { get; set; }
    public virtual Poem Poem { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;

    public override bool Equals(object? obj) =>
        obj is HaikuTag other && HaikuId == other.HaikuId && TagId == other.TagId;

    public override int GetHashCode() => HashCode.Combine(HaikuId, TagId);
}
