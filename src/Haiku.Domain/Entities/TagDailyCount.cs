namespace Haiku.Domain.Entities;

public class TagDailyCount
{
    public virtual int TagId { get; set; }
    public virtual DateOnly Date { get; set; }
    public virtual int Count { get; set; }
    public virtual Tag Tag { get; set; } = null!;

    public override bool Equals(object? obj) =>
        obj is TagDailyCount other && TagId == other.TagId && Date == other.Date;

    public override int GetHashCode() => HashCode.Combine(TagId, Date);
}
