using System.Text.RegularExpressions;
using Humanizer;

namespace Haiku.Modules.Poems.Syllables;

/// <summary>
///     Orchestrates syllable counting across a chain of <see cref="ISyllableProvider"/> implementations,
///     with built-in normalization for numerals, Roman numerals, and single letters.
/// </summary>
/// <remarks>
///     <para>
///         <b>Special token handling (before provider chain):</b>
///         <list type="bullet">
///             <item>Tokens consisting entirely of digits (optionally prefixed with '-') are expanded
///             to their spoken English form via Humanizer (<c>42</c> → <c>forty-two</c>) and the
///             total syllable count of the spoken form is returned (SC-06a).</item>
///             <item>Uppercase Roman numeral tokens are converted to their integer value, expanded
///             to spoken form, and counted (SC-06b).</item>
///             <item>Single-letter tokens are counted using the spoken letter name (SC-05).</item>
///         </list>
///     </para>
///     <para>
///         After special token normalization, the first <see cref="ISyllableProvider"/> to return
///         a result wins. If no provider matches, the word defaults to 1 syllable with tier "none".
///     </para>
///     <para>
///         <b>Homograph resolution:</b> Currently returns the first pronunciation for words with
///         multiple entries. Future work should implement context-aware selection
///         (e.g., part-of-speech tagging) at this layer.
///     </para>
/// </remarks>
public sealed partial class SyllableEngine
{
    private static readonly Regex RomanNumeralPattern = RomanNumeralRegex();
    private readonly IReadOnlyList<ISyllableProvider> _providers;
    private readonly IWordTokenizer _tokenizer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SyllableEngine"/> class
    ///     with the given provider chain and tokenizer.
    /// </summary>
    /// <param name="providers">The chain of syllable providers, tried in order. The last provider
    /// should handle any remaining alphabetic words (typically <see cref="Providers.HeuristicSyllableProvider"/>).</param>
    /// <param name="tokenizer">The word tokenizer for splitting lines into word tokens.</param>
    public SyllableEngine(IEnumerable<ISyllableProvider> providers, IWordTokenizer tokenizer)
    {
        _providers = providers.ToList();
        _tokenizer = tokenizer;
    }

    /// <summary>
    ///     Counts syllables in a single word, handling special token types before
    ///     consulting the provider chain.
    /// </summary>
    /// <param name="word">The word or token to evaluate.</param>
    /// <returns>A <see cref="SyllableResult"/> with the syllable count and resolution tier.</returns>
    public SyllableResult CountWordSyllables(string word)
    {
        // Special token normalization (SC-06a, SC-06b, SC-05)
        if (TryCountSpecialToken(word, out var specialResult))
        {
            return specialResult;
        }

        // Standard provider chain
        foreach (var provider in _providers)
        {
            if (provider.TryCountSyllables(word, out var result))
            {
                return result;
            }
        }

        return new SyllableResult(word, 1, "none");
    }

    /// <summary>
    ///     Counts syllables in a line by tokenizing and summing per-word results.
    /// </summary>
    /// <param name="line">The line to analyze.</param>
    /// <param name="lineNumber">Optional 1-based line number (default 1).</param>
    /// <returns>A <see cref="LineSyllableResult"/> with per-word and total counts.</returns>
    public LineSyllableResult CountLineSyllables(string line, int lineNumber = 1)
    {
        var tokenized = _tokenizer.Tokenize(line);
        var wordResults = tokenized.Words.Select(w => CountWordSyllables(w)).ToArray();

        return new LineSyllableResult(lineNumber, line, wordResults.Sum(r => r.Count), wordResults);
    }

    /// <summary>
    ///     Attempts to count syllables for special token types that the provider chain
    ///     cannot handle directly.
    /// </summary>
    private static bool TryCountSpecialToken(string word, out SyllableResult result)
    {
        var trimmed = word.Trim();

        // SC-06a: Numeral tokens (all digits, optionally with leading '-')
        if (IsNumeralToken(trimmed))
        {
            if (int.TryParse(trimmed, out var number))
            {
                var spoken = number.ToWords();
                var count = CountSpokenSyllables(spoken);
                result = new SyllableResult(word, count, "Numeral");
                return true;
            }

            // Fallback: count each digit separately
            result = new SyllableResult(word, trimmed.Length, "Numeral");
            return true;
        }

        // SC-06b: Roman numeral tokens (uppercase)
        if (IsRomanNumeral(trimmed))
        {
            var number = RomanNumeralsToInt(trimmed);
            var spoken = number.ToWords();
            var count = CountSpokenSyllables(spoken);
            result = new SyllableResult(word, count, "RomanNumeral");
            return true;
        }

        // Ordinal tokens: "1st" → "first", "101st" → "one hundred and first", etc.
        if (IsOrdinalToken(trimmed))
        {
            if (TryParseOrdinal(trimmed, out var number))
            {
                var spoken = number.ToOrdinalWords();
                var count = CountSpokenSyllables(spoken);
                result = new SyllableResult(word, count, "Ordinal");
                return true;
            }

            // Fallback: count each digit + letter separately
            result = new SyllableResult(word, trimmed.Length, "Ordinal");
            return true;
        }

        // SC-05: Single-letter tokens
        if (trimmed.Length == 1 && char.IsLetter(trimmed[0]))
        {
            var count = LetterNameSyllables(trimmed[0]);
            result = new SyllableResult(word, count, "LetterName");
            return true;
        }

        result = null!;
        return false;
    }

    /// <summary>
    ///     Counts syllables in the spoken English output from Humanizer.
    ///     Splits on spaces and hyphens, then estimates syllables per word
    ///     using a simple vowel-group heuristic (same algorithm as
    ///     <see cref="Providers.HeuristicSyllableProvider"/>).
    /// </summary>
    private static int CountSpokenSyllables(string spoken)
    {
        if (string.IsNullOrWhiteSpace(spoken))
        {
            return 0;
        }

        return spoken.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries).Sum(VowelGroupCount);
    }

    /// <summary>
    ///     Determines if a token is a numeral (all digits, optionally prefixed with '-').
    /// </summary>
    private static bool IsNumeralToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        if (token[0] == '-')
        {
            return token.Length > 1 && token[1..].All(char.IsDigit);
        }

        return token.All(char.IsDigit);
    }

    /// <summary>
    ///     Determines if a token is a valid uppercase Roman numeral.
    /// </summary>
    private static bool IsRomanNumeral(string token)
    {
        // Single-character Roman numerals like "I" are almost certainly the
        // English word "I" or a single letter, not the numeral. Require ≥2 chars.
        return token.Length >= 2 && RomanNumeralPattern.IsMatch(token);
    }

    /// <summary>
    ///     Determines if a token is an ordinal like "1st", "2nd", "3rd", "101st".
    /// </summary>
    private static bool IsOrdinalToken(string token)
    {
        return !string.IsNullOrEmpty(token) && OrdinalPattern.IsMatch(token);
    }

    /// <summary>
    ///     Attempts to parse the numeric value from an ordinal token.
    ///     "1st" → 1, "101st" → 101, "42nd" → 42.
    /// </summary>
    private static bool TryParseOrdinal(string token, out int value)
    {
        var digits = new string(token.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out value);
    }

    /// <summary>
    ///     Converts a Roman numeral string to its integer value using Humanizer.
    /// </summary>
    private static int RomanNumeralsToInt(string roman) => roman.FromRoman();

    /// <summary>
    ///     Returns the syllable count for a single letter based on its spoken English name.
    /// </summary>
    private static int LetterNameSyllables(char letter)
    {
        // All letters are 1 syllable except W ("double-you" = 3) and
        // H ("aitch" = 1, but some dialects say "haitch" = 1).
        return char.ToUpperInvariant(letter) == 'W' ? 3 : 1;
    }

    /// <summary>
    ///     Very simple vowel-group heuristic for counting syllables in
    ///     spoken English words from Humanizer output. This is a lightweight
    ///     alternative to the full <see cref="Providers.HeuristicSyllableProvider"/>
    ///     and is only used for Humanizer-generated words (which are always
    ///     standard English number words like "forty", "two", "thousand", etc.).
    /// </summary>
    private static int VowelGroupCount(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return 0;
        }

        const string vowels = "aeiouy";
        var count = 0;
        var lastWasVowel = false;

        foreach (var c in word)
        {
            if (vowels.Contains(c))
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

        return Math.Max(1, count);
    }

    [GeneratedRegex(@"^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$")]
    private static partial Regex RomanNumeralRegex();

    [GeneratedRegex(@"^\d+(st|nd|rd|th)$", RegexOptions.IgnoreCase)]
    private static partial Regex OrdinalPattern { get; }
}
