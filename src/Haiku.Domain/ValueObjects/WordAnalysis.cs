namespace Haiku.Domain.ValueObjects;

/// <summary>Analysis result for a single word, produced by <c>SyllableEngine</c>.</summary>
public record WordAnalysis
{
    /// <summary>Gets the word in lowercase form.</summary>
    /// <value>The lowercased word text.</value>
    public string Word { get; init; } = string.Empty;

    /// <summary>Gets the number of syllables detected in this word.</summary>
    /// <value>Zero if the word was not found in the pronunciation dictionary.</value>
    public int Syllables { get; init; }

    /// <summary>Gets the pronunciation tier used for syllable counting.</summary>
    /// <value>"A" for exact match, "B" for fallback, "C" for manual override, or "unknown".</value>
    public string Tier { get; init; } = string.Empty;
}
