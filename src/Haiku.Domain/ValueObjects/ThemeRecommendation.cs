namespace Haiku.Domain.ValueObjects;

/// <summary>
/// Represents a theme suggested by the recommendation engine for a poem.
/// </summary>
/// <remarks>
/// <para>Produced by the theme-matching service, which scores each active theme against
/// a poem's content using keyword matching. The <see cref="Confidence"/> value represents
/// the match strength. A null <see cref="ThemeId"/> indicates the poem did not match any
/// known theme at a meaningful confidence threshold.</para>
/// </remarks>
public record ThemeRecommendation
{
    /// <summary>Gets the unique identifier of the recommended theme, or <c>null</c> if no theme matched.</summary>
    /// <value>The primary key of the matched <c>Theme</c> entity, or <c>null</c>.</value>
    public Guid? ThemeId { get; init; }

    /// <summary>Gets the match confidence score between 0 and 1.</summary>
    /// <value>A score where 1.0 represents a perfect match and 0.0 represents no match.</value>
    public double Confidence { get; init; }

    /// <summary>Gets the human-readable name of the recommended theme, or <c>null</c> if not set.</summary>
    /// <value>The display name of the matched theme, or <c>null</c>.</value>
    public string? ThemeDisplayName { get; init; }
}
