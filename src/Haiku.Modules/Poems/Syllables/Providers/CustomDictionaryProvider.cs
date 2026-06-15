using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Syllables.Providers;

/// <summary>
///     Syllable provider backed by a runtime-supplied dictionary of word-to-syllable-count mappings.
///     Useful for overriding individual word pronunciations or adding domain-specific terms
///     such as usernames, slang, or platform-specific vocabulary.
/// </summary>
public sealed class CustomDictionaryProvider : ISyllableProvider
{
    private readonly Dictionary<string, int> _words;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CustomDictionaryProvider"/> class.
    /// </summary>
    /// <param name="words">
    ///     An optional dictionary mapping words to syllable counts.
    ///     Defaults to an empty dictionary if not provided.
    /// </param>
    public CustomDictionaryProvider(Dictionary<string, int>? words = null)
    {
        _words = words ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Attempts to count syllables in <paramref name="word"/> using the custom dictionary.
    /// </summary>
    /// <param name="word">The word to look up. Comparison is case-insensitive.</param>
    /// <param name="result">
    ///     When successful, contains the syllable count with tier "Custom";
    ///     <c>null</c> when the word is not in the dictionary.
    /// </param>
    /// <returns><c>true</c> if the word was found; <c>false</c> otherwise.</returns>
    public bool TryCountSyllables(string word, [NotNullWhen(true)] out SyllableResult? result)
    {
        if (_words.TryGetValue(word, out var count))
        {
            result = new SyllableResult(word, count, "Custom");
            return true;
        }

        result = null;
        return false;
    }
}
