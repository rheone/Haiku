using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Syllables.Providers;

public sealed class CmuDictionaryProvider : ISyllableProvider
{
    private readonly Dictionary<string, WordEntry> _dictionary = new(StringComparer.OrdinalIgnoreCase);

    private sealed record WordEntry(int SyllableCount, string[] Phonemes);

    public CmuDictionaryProvider(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("CMU dictionary file not found.", filePath);
        }

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";;;"))
            {
                continue;
            }

            var parts = line.Split("  ", 2, StringSplitOptions.None);
            if (parts.Length < 2)
            {
                // Simple word list format — just a word per line
                var plainWord = parts[0].Trim().ToLowerInvariant();
                if (!string.IsNullOrEmpty(plainWord))
                {
                    _dictionary.TryAdd(plainWord, new WordEntry(0, []));
                }
                continue;
            }

            var rawWord = parts[0];
            var word = rawWord.ToLowerInvariant();
            var phonemes = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var syllableCount = phonemes.Count(p => p.Any(char.IsDigit));

            _dictionary.TryAdd(word, new WordEntry(syllableCount, phonemes));
        }
    }

    public bool TryCountSyllables(string word, [NotNullWhen(true)] out SyllableResult? result)
    {
        if (_dictionary.TryGetValue(word, out var entry))
        {
            result = new SyllableResult(word, entry.SyllableCount > 0 ? entry.SyllableCount : 1, "CMU");
            return true;
        }

        result = null;
        return false;
    }

    public IReadOnlyDictionary<string, int> GetWordsBySyllableCount()
    {
        return _dictionary
            .Where(kvp => kvp.Value.SyllableCount > 0)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.SyllableCount);
    }

    public bool TryGetPhonemes(string word, [NotNullWhen(true)] out string[]? phonemes)
    {
        if (_dictionary.TryGetValue(word, out var entry) && entry.Phonemes.Length > 0)
        {
            phonemes = entry.Phonemes;
            return true;
        }

        phonemes = null;
        return false;
    }
}
