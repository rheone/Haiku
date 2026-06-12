using Haiku.Domain.Enums;

namespace Haiku.Domain.Entities;

public class Haiku
{
    public virtual Guid Id { get; set; }
    public virtual User Author { get; set; } = null!;
    public virtual string Content { get; set; } = string.Empty;
    public virtual PoemType PoemType { get; set; }
    public virtual int TotalSyllables { get; set; }
    public virtual bool IsDraft { get; set; }
    public virtual bool IsHidden { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime? DeletedAt { get; set; }

    public virtual bool IsDeleted => DeletedAt.HasValue;
}
