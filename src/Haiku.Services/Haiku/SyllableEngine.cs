namespace Haiku.Services.Haiku;

public class SyllableEngine
{
    private readonly HashSet<string> _cmuDictionary;
    private readonly Dictionary<string, int> _customDictionary;

    public SyllableEngine(
        Dictionary<string, int>? customDictionary = null,
        HashSet<string>? cmuDictionary = null)
    {
        _customDictionary = customDictionary ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        _cmuDictionary = cmuDictionary ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public int CountWordSyllables(string word)
    {
        var cleaned = word.Trim().TrimEnd('.', ',', '!', '?', ';', ':', '"', '\'', ')', ']', '}');

        if (string.IsNullOrEmpty(cleaned))
            return 0;

        if (_customDictionary.TryGetValue(cleaned, out var customCount))
            return customCount;

        if (_cmuDictionary.Contains(cleaned.ToUpperInvariant()))
        {
            return CountFromCmu(cleaned);
        }

        return VowelGroupHeuristic(cleaned);
    }

    public List<int> CountLineSyllables(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new List<int>();

        return line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(CountWordSyllables)
            .ToList();
    }

    private static int CountFromCmu(string word)
    {
        var vowels = word.ToLowerInvariant().Count(c => "aeiou".Contains(c));
        return Math.Max(vowels, 1);
    }

    private static int VowelGroupHeuristic(string word)
    {
        var lower = word.ToLowerInvariant();
        var count = 0;
        var prevVowel = false;

        foreach (var c in lower)
        {
            var isVowel = "aeiou".Contains(c);
            if (isVowel && !prevVowel)
                count++;
            prevVowel = isVowel;
        }

        if (lower.EndsWith('e') && count > 1)
            count--;

        if (lower.EndsWith("le") && lower.Length > 2 && !"aeiou".Contains(lower[^3]))
            count++;

        return Math.Max(count, 1);
    }
}
