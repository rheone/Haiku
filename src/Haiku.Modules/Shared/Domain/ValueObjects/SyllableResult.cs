namespace Haiku.Modules.Shared.Domain.ValueObjects;

/// <summary>Per-word syllable count and CMU pronunciation tier classification.</summary>
/// <param name="Word">The word as it appears in the original text (lowercased).</param>
/// <param name="Count">Number of syllables detected in the word.</param>
/// <param name="Tier">Pronunciation tier classification (e.g. "A", "B", "C", or "unknown").</param>
public record SyllableResult(string Word, int Count, string Tier);
