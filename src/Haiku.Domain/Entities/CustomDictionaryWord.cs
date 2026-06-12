namespace Haiku.Domain.Entities;

public class CustomDictionaryWord
{
    public virtual Guid Id { get; set; }
    public virtual string Word { get; set; } = string.Empty;
    public virtual int SyllableCount { get; set; }
    public virtual User AddedBy { get; set; } = null!;
    public virtual DateTime AddedAt { get; set; }
    public virtual bool IsApproved { get; set; }
}
