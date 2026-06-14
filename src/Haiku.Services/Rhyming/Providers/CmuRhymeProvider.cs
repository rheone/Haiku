using System.Diagnostics.CodeAnalysis;
using Haiku.Services.Syllables.Providers;

namespace Haiku.Services.Rhyming.Providers;

public sealed class CmuRhymeProvider : IRhymeProvider
{
    private readonly CmuDictionaryProvider _cmuProvider;

    public CmuRhymeProvider(CmuDictionaryProvider cmuProvider)
    {
        _cmuProvider = cmuProvider;
    }

    public bool TryGetRhymeKey(string word, [NotNullWhen(true)] out string? key)
    {
        if (_cmuProvider.TryGetPhonemes(word, out var phonemes))
        {
            key = BuildRhymeKey(phonemes);
            return key != null;
        }

        key = null;
        return false;
    }

    private static string? BuildRhymeKey(string[] phonemes)
    {
        var lastStress = Array.FindLastIndex(phonemes, p => p.EndsWith('1'));
        if (lastStress < 0)
        {
            lastStress = Array.FindLastIndex(phonemes, p => p.Any(char.IsDigit));
        }

        if (lastStress < 0)
        {
            return null;
        }

        return string.Join(" ", phonemes.Skip(lastStress).Select(p => p.TrimEnd('0', '1', '2')));
    }
}
