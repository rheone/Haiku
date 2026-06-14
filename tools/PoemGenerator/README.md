# PoemGenerator

Interactive TUI tool that generates random poems of any type using the Haiku platform's poem engine and CMU pronunciation dictionary.

## Prerequisites

The CMU pronunciation dictionary must be built first:

```bash
dotnet run tools/build-cmudict.cs
```

This generates `src/Haiku.Services/Resources/cmudict.json` (~24 MB, 126K entries), which the PoemGenerator links at build time.

## Usage

```bash
dotnet run --project tools/PoemGenerator
```

### Interactive flow

1. **Main Menu** — Generate Poems, Help/About, or Exit
2. **Type selection** — Choose how to select poem types:
   - All types (38)
   - Pick specific types
   - Random subset
3. **Poems per type** — How many of each (default 3)
4. **Random seed** — Optional seed for reproducible output
5. **Output mode** — Display in terminal, Save to markdown file, or Both

### Supported types

All **38 poem types** are supported for generation, including both traditional forms
(Haiku, Tanka, Sedoka, Cinquain variants, Lune variants, Choka, Isosyllabic, etc.)
and non-traditional sequence/constraint types (Pi, Fib, Wave, Prime, Hailstone,
etc.) — in both syllable-based and word-based variants.
Select "Help / About" in the TUI for a complete list.

### Output

- **Terminal display**: Poems shown in bordered panels with syllable patterns
- **Markdown export**: Saved to `generated-poems/PoemGenerator_<timestamp>_<commit>.md`
  - Includes local and UTC generation timestamps
  - Per-poem metadata (type, line count, syllable pattern, total syllables)
  - Blockquoted poem text

## File naming

Markdown files follow the pattern:
```
PoemGenerator_YYYYMMDD-HHmmss_<commit-hash>.md
```

Example: `PoemGenerator_20260614-143022_abc1234.md`

## Architecture

- **Spectre.Console** — rich TUI with interactive prompts and formatted output
- **Haiku.Services** — `PoemEngine.GeneratePoem()` for generation, `SyllableEngine` for counting
- **CmuDictionaryProvider** — 126K-word CMU pronunciation dictionary for word selection
- Manual instantiation of all services (no DI container needed)
