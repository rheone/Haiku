namespace Haiku.Domain.ValueObjects;

/// <summary>Result of syllable analysis for a single line of a poem.</summary>
/// <param name="LineNumber">The 1-based index of this line within the poem.</param>
/// <param name="Text">The raw text of the line.</param>
/// <param name="TotalSyllables">Total syllable count across all words in this line.</param>
/// <param name="Words">Per-word syllable analysis results.</param>
public record LineSyllableResult(int LineNumber, string Text, int TotalSyllables, SyllableResult[] Words);
