namespace Haiku.Services.Haiku;

/// <summary>
/// Counts syllables in words and lines using a three-tier resolution:
/// custom dictionary, CMU dictionary membership, then vowel-group heuristic.
/// </summary>
public class SyllableEngine
{
    private readonly HashSet<string> _cmuDictionary;
    private readonly Dictionary<string, int> _customDictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyllableEngine"/> class.
    /// </summary>
    /// <param name="customDictionary">Optional dictionary mapping words to explicit syllable counts. Case-insensitive keys.</param>
    /// <param name="cmuDictionary">Optional set of words known to the CMU pronunciation dictionary. Case-insensitive.</param>
    public SyllableEngine(Dictionary<string, int>? customDictionary = null, HashSet<string>? cmuDictionary = null)
    {
        _customDictionary = customDictionary ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        _cmuDictionary = cmuDictionary ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Counts the syllables in a single word using the three-tier resolution.
    /// </summary>
    /// <param name="word">The word to evaluate. Trailing punctuation is stripped before lookup.</param>
    /// <returns>The syllable count, or 0 if the word is blank.</returns>
    public virtual int CountWordSyllables(string word)
    {
        var cleaned = word.Trim().TrimEnd('.', ',', '!', '?', ';', ':', '"', '\'', ')', ']', '}');

        if (string.IsNullOrEmpty(cleaned))
        {
            return 0;
        }

        if (_customDictionary.TryGetValue(cleaned, out var customCount))
        {
            return customCount;
        }

        if (_cmuDictionary.Contains(cleaned.ToUpperInvariant()))
        {
            return VowelGroupHeuristic(cleaned);
        }

        return VowelGroupHeuristic(cleaned);
    }

    /// <summary>
    /// Counts syllables per word for each word in a line of text.
    /// </summary>
    /// <param name="line">The line of text to evaluate.</param>
    /// <returns>A list of syllable counts, one per word. Returns an empty list for blank lines.</returns>
    public virtual List<int> CountLineSyllables(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return new List<int>();
        }

        return line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(CountWordSyllables).ToList();
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
            {
                count++;
            }

            prevVowel = isVowel;
        }

        // Account for silent 'e' at end of word.
        if (lower.EndsWith('e') && count > 1)
        {
            count--;
        }

        // Account for syllabic 'le' (e.g., "table", "little").
        if (lower.EndsWith("le") && lower.Length > 2 && !"aeiou".Contains(lower[^3]))
        {
            count++;
        }

        return Math.Max(count, 1);
    }
}
