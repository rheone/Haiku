# build-cmudict

Downloads the canonical CMU Pronouncing Dictionary from `cmusphinx/cmudict` and writes a compact JSON snapshot to `src/Haiku.Services/Resources/cmudict.json`.

The CMU Pronouncing Dictionary is **public domain** — https://github.com/cmusphinx/cmudict

## Usage

```bash
dotnet run tools/build-cmudict.cs
```

Output: `src/Haiku.Services/Resources/cmudict.json` (~24 MB, ~126K words)

## Output schema

```json
{
  "_metadata": {
    "source": "https://github.com/cmusphinx/cmudict",
    "commit": "abc123def...",
    "generatedAt": "2026-06-14T12:00:00Z",
    "license": "Public Domain",
    "entryCount": 126052,
    "homographCount": 8447
  },
  "entries": {
    "hello": [{ "s": 2, "p": ["HH", "AH0", "L", "OW1"] }],
    "record": [
      { "s": 2, "p": ["R", "EH1", "K", "ER0", "D"] },
      { "s": 3, "p": ["R", "IH0", "K", "AO1", "R", "D"] }
    ]
  }
}
```

Each word maps to an array of pronunciation entries (usually 1; homographs have multiple). Each entry has:
- **`s`**: syllable count derived from stress markers (digits 0/1/2 in Arpabet)
- **`p`**: phoneme array in Arpabet notation

## Options

| Override | CLI flag | Environment variable |
|---|---|---|
| Upstream URL | `--url <url>` | `CMUDICT_URL` |
| Output path | `--output <path>` | `CMUDICT_OUTPUT` |

The default output path is relative to the tool's directory (`tools/`), so running from the repo root requires setting the env var:

```bash
CMUDICT_OUTPUT=src/Haiku.Services/Resources/cmudict.json dotnet run tools/build-cmudict.cs
```

## Consumers

- **`tools/PoemGenerator/`** — links the JSON at build time for random poem generation
- **`src/Haiku.Services/`** — `CmuDictionaryProvider` loads it at runtime for syllable counting and word lookups (content file, PreserveNewest)
