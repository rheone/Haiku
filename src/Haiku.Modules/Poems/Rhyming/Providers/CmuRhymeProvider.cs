using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Rhyming.Providers;

/// <summary>
///     Rhyme provider backed by the CMU Pronouncing Dictionary.
///     Builds rhyme keys from phoneme arrays: the key consists of all phonemes from
///     the last stressed vowel onward, with stress markers stripped.
/// </summary>
public sealed class CmuRhymeProvider : IRhymeProvider
{
    private readonly CmuDictionaryProvider _cmuProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CmuRhymeProvider"/> class.
    /// </summary>
    /// <param name="cmuProvider">The CMU dictionary provider for phoneme lookups.</param>
    public CmuRhymeProvider(CmuDictionaryProvider cmuProvider)
    {
        _cmuProvider = cmuProvider;
    }

    /// <inheritdoc/>
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
