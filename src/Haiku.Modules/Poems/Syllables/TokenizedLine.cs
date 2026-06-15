namespace Haiku.Modules.Poems.Syllables;

/// <summary>
///     Represents the result of tokenizing a single line of poem text into word tokens
///     with associated syllable counting metadata.
/// </summary>
public record TokenizedLine
{
    /// <summary>Gets the array of extracted word tokens.</summary>
    /// <value>The individual word strings from the line.</value>
    public string[] Words { get; init; } = [];

    /// <summary>Gets the syllable count for each word, in order.</summary>
    /// <value>An array parallel to <see cref="Words"/> with each word's syllable count.</value>
    public int[] WordSyllableCounts { get; init; } = [];

    /// <summary>Gets the total syllable count across all words in the line.</summary>
    /// <value>The sum of all <see cref="WordSyllableCounts"/>.</value>
    public int TotalSyllables { get; init; }

    /// <summary>Gets the number of word tokens extracted from the line.</summary>
    /// <value>The count of tokens in <see cref="Words"/>.</value>
    public int WordCount { get; init; }
}
