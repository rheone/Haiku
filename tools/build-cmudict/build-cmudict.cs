// build-cmudict.cs — CMU Pronunciation Dictionary Pre-processor
//
// Usage:
//   dotnet run --project tools/build-cmudict.cs
//
//   or (when the SDK has single-file support):
//   dotnet run tools/build-cmudict.cs
//
// This tool downloads the canonical CMU Pronouncing Dictionary from the
// cmusphinx/cmudict repository, parses it, and writes a compact JSON snapshot
// to src/Haiku.Services/Resources/cmudict.json.
//
// The CMU Pronouncing Dictionary (cmudict) is in the public domain:
//   https://github.com/cmusphinx/cmudict
//
// Output schema:
//   {
//     "_metadata": {
//       "source": "https://github.com/cmusphinx/cmudict",
//       "commit": "abc123def...",
//       "generatedAt": "2026-06-14T12:00:00Z",
//       "license": "Public Domain",
//       "entryCount": 133456,
//       "homographCount": 1234
//     },
//     "entries": {
//       "hello": [{ "s": 2, "p": ["HH", "AH0", "L", "OW1"] }],
//       "record": [
//         { "s": 2, "p": ["R", "EH1", "K", "ER0", "D"] },
//         { "s": 3, "p": ["R", "IH0", "K", "AO1", "R", "D"] }
//       ]
//     }
//   }
//
// Each word maps to an array of pronunciation entries (usually 1; homographs
// have multiple). Each entry has:
//   - "s": syllable count derived from stress markers (digits 0/1/2 in Arpabet)
//   - "p": phoneme array in Arpabet notation
//
// Upstream URL can be overridden via --url or the CMUDICT_URL env var.
// Output path can be overridden via --output or the CMUDICT_OUTPUT env var.

using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

const string DefaultUrl =
    "https://raw.githubusercontent.com/cmusphinx/cmudict/master/cmudict.dict";

const string DefaultOutputPath =
    "../src/Haiku.Services/Resources/cmudict.json";

// Parse command-line arguments
var url = DefaultUrl;
var outputPath = DefaultOutputPath;

for (var i = 0; i < args.Length; i++)
{
    if (args[i] == "--url" && i + 1 < args.Length)
        url = args[++i];
    else if (args[i] == "--output" && i + 1 < args.Length)
        outputPath = args[++i];
}

url = Environment.GetEnvironmentVariable("CMUDICT_URL") ?? url;
outputPath = Environment.GetEnvironmentVariable("CMUDICT_OUTPUT") ?? outputPath;

Console.WriteLine($"CMU Dictionary Build Tool");
Console.WriteLine($"  Source: {url}");
Console.WriteLine($"  Output: {outputPath}");
Console.WriteLine();

// Download
Console.WriteLine("Downloading CMU dictionary...");
using var http = new HttpClient();
http.DefaultRequestHeaders.Add("User-Agent", "Haiku/1.0");
var text = await http.GetStringAsync(url);
var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
Console.WriteLine($"  Downloaded {lines.Length} lines.");

// Parse
Console.WriteLine("Parsing...");
var entries = new Dictionary<string, List<PronunciationEntry>>(
    StringComparer.OrdinalIgnoreCase
);

var homographCount = 0;

foreach (var rawLine in lines)
{
    var line = rawLine.TrimEnd('\r');
    if (line.Length == 0 || line.StartsWith(";;;"))
        continue;

    // CMU format: "WORD  PHONEME1 PHONEME2 ..." (traditionally double space, but
    // the raw file on GitHub uses a single space). Handle both by splitting on
    // the first space.
    var firstSpace = line.IndexOf(' ');
    if (firstSpace < 0)
        continue;

    var raw = line[..firstSpace];
    var phonemePart = line[(firstSpace + 1)..].Trim();

    // Strip parenthetical disambiguation suffixes: "WORD(2)" -> "WORD"
    var parenIdx = raw.IndexOf('(');
    var word = parenIdx >= 0 ? raw[..parenIdx] : raw;

    var phonemes = phonemePart.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var syllableCount = phonemes.Count(p => p.Any(char.IsDigit));

    var entry = new PronunciationEntry(syllableCount, phonemes);

    if (entries.TryGetValue(word, out var existing))
    {
        // Homograph: add to existing entry list
        existing.Add(entry);
    }
    else
    {
        entries[word] = [entry];
    }
}

// Count homographs
foreach (var kvp in entries)
{
    if (kvp.Value.Count > 1)
        homographCount++;
}

Console.WriteLine($"  Words: {entries.Count}, Homographs: {homographCount}");

// Build output
var output = new CmuDictFile
{
    Metadata = new Metadata
    {
        Source = "https://github.com/cmusphinx/cmudict",
        Commit = "master",
        GeneratedAt = DateTime.UtcNow.ToString("O"),
        License = "Public Domain",
        EntryCount = entries.Count,
        HomographCount = homographCount,
    },
    Entries = entries.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value.ToArray(),
        StringComparer.OrdinalIgnoreCase
    ),
};

// Ensure output directory exists
var outputDir = Path.GetDirectoryName(Path.GetFullPath(outputPath));
if (!string.IsNullOrEmpty(outputDir))
    Directory.CreateDirectory(outputDir);

// Write
var json = JsonSerializer.Serialize(output, new JsonSerializerOptions
{
    WriteIndented = true,
    TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
});

await File.WriteAllTextAsync(outputPath, json);
Console.WriteLine($"  Written: {outputPath} ({json.Length:N0} bytes)");
Console.WriteLine("Done.");

// =========================================================================
// Types
// =========================================================================

public sealed record PronunciationEntry(
    [property: JsonPropertyName("s")] int SyllableCount,
    [property: JsonPropertyName("p")] string[] Phonemes
);

public sealed record Metadata
{
    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    [JsonPropertyName("commit")]
    public string Commit { get; init; } = string.Empty;

    [JsonPropertyName("generatedAt")]
    public string GeneratedAt { get; init; } = string.Empty;

    [JsonPropertyName("license")]
    public string License { get; init; } = string.Empty;

    [JsonPropertyName("entryCount")]
    public int EntryCount { get; init; }

    [JsonPropertyName("homographCount")]
    public int HomographCount { get; init; }
}

public sealed record CmuDictFile
{
    [JsonPropertyName("_metadata")]
    public Metadata Metadata { get; init; } = new();

    [JsonPropertyName("entries")]
    public Dictionary<string, PronunciationEntry[]> Entries { get; init; } = new(
        StringComparer.OrdinalIgnoreCase
    );
}
