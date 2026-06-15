using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Syllables.Providers;

/// <summary>
///     Last-resort syllable provider that uses English pronunciation heuristics
///     for words not found in any dictionary-based provider.
/// </summary>
/// <remarks>
///     <para>
///         <b>Algorithm:</b> vowel groups (consecutive vowels counted as one) minus
///         silent-e at end of word, with special handling for consonant+le endings
///         (e.g., "table", "people") and single-letter words.
///     </para>
///     <para>
///         This provider is a <b>fallback only</b> — it fires for neologisms, proper nouns,
///         slang, and obscure words not present in the CMU pronunciation dictionary.
///         The algorithm is intentionally simple and may be refined over time.
///     </para>
///     <para>
///         <b>Known limitations:</b>
///         <list type="bullet">
///             <item>Does not normalize diphthongs — adjacent vowel letters are always
///             counted as a single group, which is correct for most cases but overcounts
///             for some vowel sequences across syllable boundaries.</item>
///             <item>Does not handle compound words or affixes as separate syllables.</item>
///         </list>
///     </para>
/// </remarks>
public sealed class HeuristicSyllableProvider : ISyllableProvider
{
    private const string Vowels = "aeiouy";

    /// <summary>
    ///     Attempts to count syllables in <paramref name="word"/> using English heuristics.
    /// </summary>
    /// <param name="word">The word to evaluate. Non-letter characters are stripped.</param>
    /// <param name="result">
    ///     When successful, contains the syllable count with tier "Heuristic".
    ///     <c>null</c> when the word contains no alphabetic characters.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the word contains at least one letter and a count was computed;
    ///     <c>false</c> for empty, whitespace, or non-alphabetic input.
    /// </returns>
    public bool TryCountSyllables(string word, [NotNullWhen(true)] out SyllableResult? result)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            result = null;
            return false;
        }

        var clean = new string([.. word.Where(char.IsLetter)]).ToLowerInvariant();

        if (string.IsNullOrEmpty(clean))
        {
            result = null;
            return false;
        }

        var count = VowelGroupCount(clean);
        result = new SyllableResult(word, count, "Heuristic");
        return true;
    }

    /// <summary>
    ///     Determines whether a word ends with a consonant + "le" pattern,
    ///     which exempts the final 'e' from silent-e subtraction.
    /// </summary>
    /// <param name="word">The lowercase, letter-only word to check.</param>
    /// <returns>
    ///     <c>true</c> if the word ends with a consonant followed by "le"
    ///     (e.g., "table", "people", "cradle", "bottle");
    ///     <c>false</c> otherwise (e.g., "ale" has a vowel before "le").
    /// </returns>
    private static bool IsConsonantLe(string word)
    {
        return word.Length >= 3 && word[^2..] == "le" && !Vowels.Contains(word[^3]);
    }

    /// <summary>
    ///     Counts syllables using the vowel-group heuristic.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Vowel-group rule: each maximal sequence of consecutive vowel letters
    ///         (a, e, i, o, u, y) is counted as one syllable. Single-letter words
    ///         always produce 1. Silent 'e' at the end of a word subtracts one from
    ///         the total unless the word ends in a consonant+le pattern.
    ///     </para>
    /// </remarks>
    /// <param name="word">Lowercase word containing only letters.</param>
    /// <returns>The heuristic syllable count, always at least 1.</returns>
    private static int VowelGroupCount(string word)
    {
        if (word.Length == 1)
        {
            return 1;
        }

        var count = 0;
        var lastWasVowel = false;
        var isConsonantLe = IsConsonantLe(word);

        foreach (var c in word)
        {
            if (Vowels.Contains(c))
            {
                if (!lastWasVowel)
                {
                    count++;
                }

                lastWasVowel = true;
            }
            else
            {
                lastWasVowel = false;
            }
        }

        if (word.Length > 2 && word.EndsWith('e') && !Vowels.Contains(word[^2]) && !isConsonantLe)
        {
            count--;
        }

        return Math.Max(1, count);
    }
}
