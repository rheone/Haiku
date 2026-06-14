using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Syllables.Providers;

public sealed class HeuristicSyllableProvider : ISyllableProvider
{
    private const string Vowels = "aeiouy";

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

    private static int VowelGroupCount(string word)
    {
        var count = 0;
        var lastWasVowel = false;

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

        if (word.Length > 2 && word.EndsWith('e') && !Vowels.Contains(word[^2]))
        {
            count--;
        }

        return Math.Max(1, count);
    }
}
