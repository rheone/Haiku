namespace Haiku.Domain.ValueObjects;

public record ThemeRecommendation
{
    public Guid? ThemeId { get; init; }
    public double Confidence { get; init; }
    public string? ThemeDisplayName { get; init; }
}
