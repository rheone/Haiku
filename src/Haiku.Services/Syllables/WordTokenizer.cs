using System.Text.RegularExpressions;

namespace Haiku.Services.Syllables;

/// <summary>
///     Default implementation of <see cref="IWordTokenizer"/> that splits lines on whitespace,
///     removes non-spoken characters via regex, and retains only valid word tokens
///     (alphabetic sequences, numerals, Roman numerals, and ordinals).
/// </summary>
public sealed partial class WordTokenizer : IWordTokenizer
{
    /// <summary>
    ///     Tokenizes a line by splitting on whitespace and filtering to valid word tokens.
    /// </summary>
    /// <param name="line">The line of text to tokenize.</param>
    /// <returns>A <see cref="TokenizedLine"/> with the extracted words and counts.</returns>
    public TokenizedLine Tokenize(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return new TokenizedLine();
        }

        var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var words = new List<string>();

        foreach (var token in tokens)
        {
            var cleaned = RemoveNonSpokenChars().Replace(token, string.Empty);
            if (IsWordToken(cleaned))
            {
                words.Add(cleaned);
            }
        }

        return new TokenizedLine { Words = [.. words], WordCount = words.Count };
    }

    private static bool IsWordToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        // (a) one or more consecutive alphabetical characters
        if (token.All(char.IsLetter))
        {
            return true;
        }

        // (b) one or more consecutive numeral digits
        if (token.All(char.IsDigit))
        {
            return true;
        }

        // (c) an uppercase Roman numeral sequence
        if (IsRomanNumeral(token))
        {
            return true;
        }

        // (d) an ordinal like "1st", "2nd", "3rd", "101st" (digits + st/nd/rd/th)
        if (IsOrdinalToken(token))
        {
            return true;
        }

        return false;
    }

    private static bool IsRomanNumeral(string token)
    {
        // Single-character Roman numerals like "I" are almost certainly the
        // English word "I" or a single letter, not the numeral. Require ≥2 chars.
        return token.Length >= 2 && RomanNumeralRegex().IsMatch(token);
    }

    private static bool IsOrdinalToken(string token)
    {
        return OrdinalPattern().IsMatch(token);
    }

    [GeneratedRegex(@"^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$")]
    private static partial Regex RomanNumeralRegex();

    [GeneratedRegex(@"^\d+(st|nd|rd|th)$", RegexOptions.IgnoreCase)]
    private static partial Regex OrdinalPattern();

    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    private static partial Regex RemoveNonSpokenChars();
}
