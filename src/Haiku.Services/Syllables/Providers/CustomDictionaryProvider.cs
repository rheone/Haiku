using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Syllables.Providers;

public sealed class CustomDictionaryProvider : ISyllableProvider
{
    private readonly Dictionary<string, int> _words;

    public CustomDictionaryProvider(Dictionary<string, int>? words = null)
    {
        _words = words ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    }

    public bool TryCountSyllables(string word, [NotNullWhen(true)] out SyllableResult? result)
    {
        if (_words.TryGetValue(word, out var count))
        {
            result = new SyllableResult(word, count, "Custom");
            return true;
        }

        result = null;
        return false;
    }
}
