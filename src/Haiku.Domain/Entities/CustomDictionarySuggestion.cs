using Haiku.Domain.Enums;

namespace Haiku.Domain.Entities;

public class CustomDictionarySuggestion
{
    public virtual Guid Id { get; set; }
    public virtual string Word { get; set; } = string.Empty;
    public virtual int SuggestedSyllableCount { get; set; }
    public virtual User SuggestedBy { get; set; } = null!;
    public virtual string? Justification { get; set; }
    public virtual User? ReviewedBy { get; set; }
    public virtual DateTime? ReviewedAt { get; set; }
    public virtual DictionarySuggestionStatus Status { get; set; }
}
