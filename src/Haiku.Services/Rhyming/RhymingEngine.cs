namespace Haiku.Services.Rhyming;

public sealed class RhymingEngine
{
    private readonly IRhymeProvider[] _providers;

    public RhymingEngine(IEnumerable<IRhymeProvider> providers)
    {
        _providers = providers.ToArray();
    }

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
