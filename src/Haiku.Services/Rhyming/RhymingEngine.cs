namespace Haiku.Services.Rhyming;

/// <summary>
///     Determines whether words or lines rhyme by consulting registered
///     <see cref="IRhymeProvider"/> instances, falling back to a suffix-based
///     heuristic when no provider produces a result.
/// </summary>
public sealed class RhymingEngine
{
    private readonly IRhymeProvider[] _providers;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RhymingEngine"/> class.
    /// </summary>
    /// <param name="providers">The rhyme providers to consult, in priority order.</param>
    public RhymingEngine(IEnumerable<IRhymeProvider> providers)
    {
        _providers = providers.ToArray();
    }

    /// <summary>
    ///     Determines whether two words rhyme.
    /// </summary>
    /// <param name="word1">The first word.</param>
    /// <param name="word2">The second word.</param>
    /// <returns>
    ///     <c>true</c> if the words produce the same rhyme key from any registered
    ///     provider, or if they match the suffix fallback heuristic; <c>false</c> otherwise.
    /// </returns>
    public bool WordsRhyme(string word1, string word2)
    {
        if (string.Equals(word1, word2, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        foreach (var provider in _providers)
        {
            var hasKey1 = provider.TryGetRhymeKey(word1, out var key1);
            var hasKey2 = provider.TryGetRhymeKey(word2, out var key2);

            if (hasKey1 && hasKey2)
            {
                return key1 == key2;
            }
        }

        // Fallback: compare last 2 characters as suffix heuristic
        return SuffixFallback(word1, word2);
    }

    /// <summary>
    ///     Determines whether the last words of two lines rhyme.
    /// </summary>
    /// <param name="line1">The first line of text.</param>
    /// <param name="line2">The second line of text.</param>
    /// <returns>
    ///     <c>true</c> if both lines end with a rhyming word; <c>false</c> if either
    ///     line is empty, punctuation-only, or the end-words do not rhyme.
    /// </returns>
    public bool LinesRhyme(string line1, string line2)
    {
        var w1 = GetLastWord(line1);
        var w2 = GetLastWord(line2);
        return w1 != null && w2 != null && WordsRhyme(w1, w2);
    }

    private static bool SuffixFallback(string word1, string word2)
    {
        var lower1 = word1.ToLowerInvariant();
        var lower2 = word2.ToLowerInvariant();
        return lower1.Length >= 3 && lower2.Length >= 3 && lower1[^2..] == lower2[^2..];
    }

    private static string? GetLastWord(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }

        var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length > 0 ? new string([.. words[^1].Where(char.IsLetter)]).ToLowerInvariant() : null;
    }
}
