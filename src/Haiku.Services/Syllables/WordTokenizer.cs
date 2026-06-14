using System.Text.RegularExpressions;

namespace Haiku.Services.Syllables;

public sealed partial class WordTokenizer : IWordTokenizer
{
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

        return false;
    }

    private static bool IsRomanNumeral(string token)
    {
        return RomanNumeralRegex().IsMatch(token);
    }

    [GeneratedRegex(@"^M{0,3}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$")]
    private static partial Regex RomanNumeralRegex();

    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    private static partial Regex RemoveNonSpokenChars();
}
