using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Syllables;

/// <summary>
///     Provides syllable counts for individual words.
///     Implementations may use dictionary lookups, heuristic algorithms, or other strategies.
/// </summary>
public interface ISyllableProvider
{
    /// <summary>
    ///     Attempts to count syllables in the specified word.
    /// </summary>
    /// <param name="word">The word to evaluate.</param>
    /// <param name="result">
    ///     When successful, contains the syllable count and resolution tier;
    ///     <c>null</c> when the word is not recognized by this provider.
    /// </param>
    /// <returns><c>true</c> if the word was recognized; <c>false</c> otherwise.</returns>
    bool TryCountSyllables(string word, [NotNullWhen(true)] out SyllableResult? result);
}
