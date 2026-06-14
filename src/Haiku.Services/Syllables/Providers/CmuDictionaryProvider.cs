using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Syllables.Providers;

/// <summary>
///     Syllable provider backed by the CMU Pronouncing Dictionary, pre-processed to JSON.
/// </summary>
/// <remarks>
///     <para>
///         Loads a pre-processed JSON snapshot of the CMU pronunciation dictionary at construction time
///         and keeps it in memory. The JSON is produced by <c>tools/build-cmudict.cs</c> from the canonical
///         <c>cmusphinx/cmudict</c> upstream source (public domain).
///     </para>
///     <para>
///         <b>Homograph handling:</b> Words with multiple pronunciations store all entries in an array.
///         <c>TryCountSyllables</c> returns the <b>first</b> entry. Future work should implement
///         context-aware selection (e.g., part-of-speech tagging or neighbor analysis) in the
///         <c>SyllableEngine</c> layer.
///     </para>
/// </remarks>
public sealed class CmuDictionaryProvider : ISyllableProvider
{
    private readonly Dictionary<string, PronunciationEntry[]> _entries;

    private sealed record CmuDictionaryFile(
        [property: JsonPropertyName("entries")] Dictionary<string, PronunciationEntry[]>? Entries
    );

    /// <summary>
    ///     A single pronunciation entry from the CMU dictionary.
    /// </summary>
    /// <param name="SyllableCount">Number of syllables (counted from stress markers).</param>
    /// <param name="Phonemes">Arpabet phoneme array (e.g., ["HH", "AH0", "L", "OW1"]).</param>
    public sealed record PronunciationEntry(
        [property: JsonPropertyName("s")] int SyllableCount,
        [property: JsonPropertyName("p")] string[] Phonemes
    );

    /// <summary>
    ///     Initializes a new instance from a pre-processed CMU dictionary JSON file.
    /// </summary>
    /// <param name="filePath">Path to the JSON dictionary file.</param>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="JsonException">The file contains invalid JSON or is not a valid CMU dictionary.</exception>
    /// <exception cref="InvalidOperationException">The file exists but contains no dictionary entries.</exception>
    public CmuDictionaryProvider(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"CMU pronunciation dictionary not found at expected path: '{filePath}'. "
                    + "Ensure the pre-processed cmudict.json file exists in the output directory. "
                    + "See tools/build-cmudict.cs for the build step if it is missing.",
                filePath
            );
        }

        var json = File.ReadAllText(filePath);

        CmuDictionaryFile? doc;
        try
        {
            doc = JsonSerializer.Deserialize<CmuDictionaryFile>(json);
        }
        catch (JsonException ex)
        {
            throw new JsonException(
                $"Failed to deserialize CMU pronunciation dictionary at '{filePath}'. "
                    + "The file may be corrupt or not a valid JSON dictionary. "
                    + "Re-run tools/build-cmudict.cs to regenerate it from the upstream CMU dictionary source.",
                ex
            );
        }

        var raw = doc?.Entries;

        if (raw is null || raw.Count == 0)
        {
            throw new InvalidOperationException(
                $"CMU pronunciation dictionary at '{filePath}' contains no entries. "
                    + "The dictionary is empty. "
                    + "Re-run tools/build-cmudict.cs against the upstream CMU dictionary source to generate a valid dictionary file."
            );
        }

        _entries = new Dictionary<string, PronunciationEntry[]>(raw, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Attempts to count syllables in <paramref name="word"/> using the CMU dictionary.
    /// </summary>
    /// <param name="word">The word to look up. Comparison is case-insensitive.</param>
    /// <param name="result">
    ///     When successful, contains the syllable count and phoneme tier "CMU".
    ///     <c>null</c> when the word is not in the dictionary.
    /// </param>
    /// <returns><c>true</c> if the word was found; <c>false</c> otherwise.</returns>
    public bool TryCountSyllables(string word, [NotNullWhen(true)] out SyllableResult? result)
    {
        if (_entries.TryGetValue(word, out var entries) && entries.Length > 0)
        {
            result = new SyllableResult(word, entries[0].SyllableCount, "CMU");
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    ///     Attempts to retrieve the phoneme arrays for a word.
    /// </summary>
    /// <param name="word">The word to look up. Case-insensitive.</param>
    /// <param name="phonemes">When successful, the first entry's phoneme array.</param>
    /// <returns><c>true</c> if the word was found with phoneme data; <c>false</c> otherwise.</returns>
    public bool TryGetPhonemes(string word, [NotNullWhen(true)] out string[]? phonemes)
    {
        if (_entries.TryGetValue(word, out var entries) && entries.Length > 0 && entries[0].Phonemes.Length > 0)
        {
            phonemes = entries[0].Phonemes;
            return true;
        }

        phonemes = null;
        return false;
    }

    /// <summary>
    ///     Attempts to get all words that have exactly <paramref name="syllableCount"/> syllables.
    /// </summary>
    /// <param name="syllableCount">The target syllable count (must be > 0).</param>
    /// <param name="words">When successful, the matching words.</param>
    /// <returns><c>true</c> if at least one word was found; <c>false</c> otherwise.</returns>
    public bool TryGetWordsBySyllableCount(int syllableCount, out List<string> words)
    {
        words = _entries
            .Where(kvp => kvp.Value.Length > 0 && kvp.Value[0].SyllableCount == syllableCount)
            .Select(kvp => kvp.Key)
            .ToList();

        return words.Count > 0;
    }

    /// <summary>
    ///     Attempts to get <paramref name="count"/> random words from the dictionary.
    /// </summary>
    /// <param name="count">Number of words to return (must be > 0).</param>
    /// <param name="words">When successful, the randomly selected words (order is random).</param>
    /// <param name="rng">Optional random number generator for deterministic results.</param>
    /// <returns><c>true</c> if at least one word was found; <c>false</c> otherwise.</returns>
    public bool TryGetWords(int count, out List<string> words, Random? rng = null)
    {
        rng ??= Random.Shared;
        var all = _entries.Keys.ToList();
        if (all.Count == 0)
        {
            words = [];
            return false;
        }

        var take = Math.Min(count, all.Count);
        words = [.. all.OrderBy(_ => rng.Next()).Take(take)];
        return words.Count > 0;
    }

    /// <summary>
    ///     Attempts to get one random word for each specified syllable count, in order.
    /// </summary>
    /// <param name="syllableCounts">The desired syllable counts, one per position.</param>
    /// <param name="words">When successful, one word per requested syllable count.</param>
    /// <param name="rng">Optional random number generator for deterministic results.</param>
    /// <returns><c>true</c> if a word was found for every requested syllable count; <c>false</c> otherwise.</returns>
    public bool TryGetSpecificWords(int[] syllableCounts, out List<string> words, Random? rng = null)
    {
        rng ??= Random.Shared;
        words = [];

        foreach (var count in syllableCounts)
        {
            if (!TryGetWordsBySyllableCount(count, out var candidates) || candidates.Count == 0)
            {
                words = [];
                return false;
            }

            words.Add(candidates[rng.Next(candidates.Count)]);
        }

        return true;
    }
}
