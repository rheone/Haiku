# Engine & Classifier Refactoring Plan

## Overview

Refactor the CMU dictionary, syllable tooling, poem engine, and classifier infrastructure to establish clear separation of concerns. Eliminates the `PoemEngine` god class, the duplicate detection logic across 3+ locations, and the rigid `PoemDefinition` that can't represent complex poem types.

---

## Phase 0: Update PRDs

- **`prd.md`** — Update Section 5.3 (Syllable Counting Engine), Section 8.5 (Syllable Engine Integration), Section 8.7 (Poem Type Detection) to reflect:
  - `IWordTokenizer` handling SC-10/SC-11 word rules
  - `ISyllableProvider` chain (Custom → CMU → Heuristic)
  - `IPoemClassifier` chain replacing hardcoded definitions
  - `RhymingEngine` as separate component
  - `PoemDefinition` as classification result container
- **`prd/prd.ArchitectureAddendum.md`** (new) — Document the engine architecture, provider pattern, classifier chain, word tokenization rules

---

## Phase 1: Domain Types (`Haiku.Domain`)

| Action | File | Notes |
|--------|------|-------|
| Update | `ValueObjects/PoemDefinition.cs` | Replace with classification result: Type, LineCount, SyllablesPerLine[], TotalSyllableCount, WordCountPerLine[], TotalWordCount, OriginalContent, NormalizedContent, Theme?, Metadata? |
| Move + rename | `ValueObjects/WordAnalysis.cs` | Extract from PoemEngine nested record, add to Domain |
| Create | `ValueObjects/SyllableResult.cs` | `record SyllableResult(string Word, int Count, string Tier)` |
| Create | `ValueObjects/LineSyllableResult.cs` | `record LineSyllableResult(int LineNumber, string Text, int TotalSyllables, SyllableResult[] Words)` |

### `PoemDefinition` — New Shape

```csharp
public record PoemDefinition
{
    public PoemType Type { get; init; }
    public int LineCount { get; init; }
    public int[] SyllablesPerLine { get; init; }
    public int TotalSyllableCount { get; init; }
    public int[] WordCountPerLine { get; init; }
    public int TotalWordCount { get; init; }
    public string OriginalContent { get; init; }
    public string NormalizedContent { get; init; }
    public string? Theme { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
}
```

---

## Phase 2: Syllable Tooling (`Haiku.Services.Syllables/`)

### Interfaces

**`ISyllableProvider`**
```csharp
public interface ISyllableProvider
{
    bool TryCountSyllables(string word, [NotNullWhen(true)] out SyllableResult? result);
}
```

**`IWordTokenizer`**
```csharp
public interface IWordTokenizer
{
    TokenizedLine Tokenize(string line);
}
```

Where `TokenizedLine` contains:
- `string[] Words` — the identified word tokens
- `int[] WordSyllableCounts` — per-word counts
- `int TotalSyllables`
- `int WordCount`

### Files

| Action | File | Notes |
|--------|------|-------|
| Create | `ISyllableProvider.cs` | `bool TryCountSyllables(string word, out SyllableResult?)` |
| Create | `IWordTokenizer.cs` | `TokenizedLine Tokenize(string line)` |
| Create | `WordTokenizer.cs` | SC-10 non-spoken chars, SC-11 word tokens, Humanizer numeral conversion, Roman numeral detection, letter syllable counting |
| Create | `Providers/CmuDictionaryProvider.cs` | Loads CMU dict from `dictionary.dic`, parses phonemes, counts from stress markers. Exposes phoneme data for rhyming engine |
| Create | `Providers/CustomDictionaryProvider.cs` | Interface to DB `CustomDictionaryWords` table, in-memory cache |
| Create | `Providers/HeuristicSyllableProvider.cs` | Vowel-group heuristic (extracted from `SyllableEngine.VowelGroupHeuristic`) |
| Refactor | `SyllableEngine.cs` | Orchestrator: holds `IEnumerable<ISyllableProvider>`, iterates in provider order, first non-null wins, Heuristic fallback |
| Delete | `Haiku/SyllableEngine.cs` | Moved to `Syllables/SyllableEngine.cs` |

### SyllableEngine Behavior

```csharp
public class SyllableEngine
{
    // Iterates providers in registration order; first non-null return wins
    public SyllableResult CountWordSyllables(string word)
    // Tokenizes line via IWordTokenizer, then counts per word
    public LineSyllableResult CountLineSyllables(string line)
}
```

---

## Phase 3: Rhyming (`Haiku.Services.Rhyming/`)

| Action | File | Notes |
|--------|------|-------|
| Create | `IRhymeProvider.cs` | `bool TryGetRhymeKey(string word, [NotNullWhen(true)] out string? key)` |
| Create | `Providers/CmuRhymeProvider.cs` | Builds rhyme key from last stressed vowel onwards, strips stress markers. Uses `CmuDictionaryProvider` for phoneme data |
| Create | `RhymingEngine.cs` | `WordsRhyme(word1, word2)`, `LinesRhyme(line1, line2)`. Iterates `IRhymeProvider[]`, suffix fallback |

---

## Phase 4: Poem Classifiers (`Haiku.Services.Poems.Classifiers/`)

### Interface

```csharp
public interface IPoemClassifier
{
    int Priority { get; }
    bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition);
}
```

### Chain

**`PoemClassifierChain`** — Orders by `Priority`, **throws on duplicate priority** at construction time, iterates in order, first match wins. `FreeformClassifier` is the fallback at `int.MaxValue`.

### Priority Order (step 100)

| Priority | Classifier | Rules |
|----------|-----------|-------|
| 100 | MonokuClassifier | 1 line, total syllables 4–17 |
| 200 | HaikuClassifier | 3 lines, 5-7-5 |
| 300 | KatautaClassifier | 3 lines, 5-7-7 |
| 400 | AmericanLuneClassifier | 3 lines, 3-5-3 |
| 500 | KellyLuneClassifier | 3 lines, 5-3-5 |
| 600 | CompressedClassifier | 3 lines, 2-3-2 |
| 700 | NearTraditionalClassifier | 3 lines, 4-6-4 |
| 800 | TankaClassifier | 5 lines, 5-7-5-7-7 |
| 900 | AmericanCinquainClassifier | 5 lines, 2-4-6-8-2 |
| 1000 | ReverseCinquainClassifier | 5 lines, 2-8-6-4-2 |
| 1100 | SedokaClassifier | 6 lines, 5-7-7-5-7-7 |
| 1200 | ButterflyCinquainClassifier | 9 lines, 2-4-6-8-2-8-6-4-2 |
| 1300 | MirrorCinquainClassifier | 10 lines, 2-4-6-8-2-2-8-6-4-2 |
| 1400 | ChokaClassifier | >=7 odd lines, alternating 5-7, ends 5-7-7 |
| 1500 | IsosyllabicClassifier | >=2 lines, all equal syllable count |
| MAX | FreeformClassifier | Fallback — always matches, no constraints |

### Replace

| Delete | Create |
|--------|--------|
| `Matchers/IPoemMatcher.cs` | `Classifiers/IPoemClassifier.cs` |
| `Matchers/IPoemMatcherChain.cs` | `Classifiers/PoemClassifierChain.cs` |
| `Matchers/PoemMatcherChain.cs` | (replaced by chain) |
| `Matchers/MonokuMatcher.cs` | `Classifiers/MonokuClassifier.cs` |
| `Matchers/HaikuMatcher.cs` | `Classifiers/HaikuClassifier.cs` |
| ... 13 more matchers | ... 13 more classifiers + FreeformClassifier |

---

## Phase 5: PoemEngine Refactor (`Haiku.Services.Poems/`)

### Remove
- `Definitions` dictionary (all 16 static entries)
- `IsCmuLoaded`, `LoadCmuDict(string filePath)`
- `CountLineSyllables(string)`, `CountWordSyllables(string)`
- `IsValidPoem(PoemType, ...)`, `IsValidPoem(PoemDefinition, ...)`
- `DetectPoemType(params string[])`
- `ValidateAgainstDefinition`, `ValidateIsosyllabic`, `ValidateChoka`
- `WordsRhyme`, `LinesRhyme`, `BuildRhymeKey`, `GetLastWord`
- `VowelGroupCount`, `CountWordSyllablesStatic`
- `CmuDictRawWordExtraction` regex
- `ResolveWithTier`
- `PoemAnalysis`, `LineAnalysis`, `WordAnalysis` nested records (moved to Domain)

### Keep
- `GeneratePoem(PoemType, int? seed)` — refactored to use `SyllableEngine` for word lookups instead of internal `_wordsBySyllableCount`
- `Analyze(params string[])` — uses `SyllableEngine` + `PoemClassifierChain`

### New Dependencies
- `SyllableEngine` (instead of internal CMU cache)
- `PoemClassifierChain` (for analysis type detection)

---

## Phase 6: Remove Duplicate Detection Code

| Action | File | Notes |
|--------|------|-------|
| Remove | `PoemService.DetectPoemType(string, List<int>)` | Static method — exact duplicate of `DetectPoemTypeQueryHandler.StaticDetect` |
| Remove | `DetectPoemTypeQueryHandler.StaticDetect(string, List<int>)` | Identical hardcoded pattern matching |
| Update | `DetectPoemTypeQueryHandler` | Uses `PoemClassifierChain` instead |
| Update | `PoemService` | Depends on `IPoemInputService` + `IPoemRepository` + `ITagRepository`. Remove `PoemEngine` dependency |

---

## Phase 7: DI Updates (`Program.cs`)

### Remove These Registrations

```csharp
builder.Services.AddSingleton<SyllableEngine>();           // replaced
builder.Services.AddSingleton<PoemEngine>();               // refactored registration
builder.Services.AddScoped<IPoemMatcher, MonokuMatcher>(); // replaced by classifiers
builder.Services.AddScoped<IPoemMatcher, HaikuMatcher>();  // ...
// ... 13 more matcher registrations ...
builder.Services.AddScoped<IPoemMatcherChain, PoemMatcherChain>(); // replaced
```

### Add These Registrations

```csharp
// Syllable tooling
builder.Services.AddSingleton<IWordTokenizer, WordTokenizer>();
builder.Services.AddSingleton<ISyllableProvider, CmuDictionaryProvider>();
builder.Services.AddSingleton<ISyllableProvider, CustomDictionaryProvider>();
builder.Services.AddSingleton<ISyllableProvider, HeuristicSyllableProvider>();
builder.Services.AddSingleton<SyllableEngine>();

// Rhyming
builder.Services.AddSingleton<IRhymeProvider, CmuRhymeProvider>();
builder.Services.AddSingleton<RhymingEngine>();

// Poem engine
builder.Services.AddSingleton<PoemEngine>();

// Classifiers (order matters for detection)
builder.Services.AddSingleton<IPoemClassifier, MonokuClassifier>();
builder.Services.AddSingleton<IPoemClassifier, HaikuClassifier>();
builder.Services.AddSingleton<IPoemClassifier, KatautaClassifier>();
builder.Services.AddSingleton<IPoemClassifier, AmericanLuneClassifier>();
builder.Services.AddSingleton<IPoemClassifier, KellyLuneClassifier>();
builder.Services.AddSingleton<IPoemClassifier, CompressedClassifier>();
builder.Services.AddSingleton<IPoemClassifier, NearTraditionalClassifier>();
builder.Services.AddSingleton<IPoemClassifier, TankaClassifier>();
builder.Services.AddSingleton<IPoemClassifier, AmericanCinquainClassifier>();
builder.Services.AddSingleton<IPoemClassifier, ReverseCinquainClassifier>();
builder.Services.AddSingleton<IPoemClassifier, SedokaClassifier>();
builder.Services.AddSingleton<IPoemClassifier, ButterflyCinquainClassifier>();
builder.Services.AddSingleton<IPoemClassifier, MirrorCinquainClassifier>();
builder.Services.AddSingleton<IPoemClassifier, ChokaClassifier>();
builder.Services.AddSingleton<IPoemClassifier, IsosyllabicClassifier>();
builder.Services.AddSingleton<IPoemClassifier, FreeformClassifier>();
builder.Services.AddSingleton<PoemClassifierChain>();
```

---

## Phase 8: Production Code Updates

### Files to Update
| File | Changes |
|------|---------|
| `PoemInputService.cs` | Use `PoemClassifierChain` instead of `IPoemMatcherChain`. Use `SyllableEngine` + `IWordTokenizer` |
| `PoemInputResult.cs` | Include `PoemDefinition?` |
| `IPoemInputService.cs` | Update return type if needed |
| `PoemService.cs` | Remove `PoemEngine` dep, `static DetectPoemType`, use `IPoemInputService` |
| `CreatePoemCommandHandler.cs` | Update to use new `PoemInputResult.PoemDefinition` |
| `DetectPoemTypeQuery.cs` | No changes needed |
| `DetectPoemTypeQueryHandler.cs` | Remove `StaticDetect`, use `PoemClassifierChain` |
| `Haiku.Web/Program.cs` | Update DI registrations |

---

## Phase 9: Test Changes

### Files to Create
| File | Contents |
|------|----------|
| `Haiku.Services.Tests/Syllables/WordTokenizerTests.cs` | SC-10/SC-11 rules, numerals, Roman numerals, non-spoken chars |
| `Haiku.Services.Tests/Syllables/SyllableEngineTests.cs` | Provider chain orchestration, fallback behavior |
| `Haiku.Services.Tests/Syllables/CmuDictionaryProviderTests.cs` | CMU dict loading, phoneme parsing |
| `Haiku.Services.Tests/Rhyming/RhymingEngineTests.cs` | Word rhyme, line rhyme |
| `Haiku.Services.Tests/Poems/Classifiers/*.cs` | 16 classifier test files (same patterns as existing matcher tests) |

### Files to Update
| File | Changes |
|------|---------|
| `Haiku.Tests/SyllableEngineTests.cs` | Move to `Haiku.Services.Tests/Syllables/`, update for new API |
| `Haiku.Tests/PoemServiceTests.cs` | Remove tests for removed static `DetectPoemType(content, counts)` |
| `Haiku.Tests/Slices/Poems/DetectPoemTypeQueryHandlerTests.cs` | Update for new handler (no StaticDetect) |
| `Haiku.Services.Tests/Poems/PoemInputServiceTests.cs` | Update injected dependencies |
| `Haiku.Services.Tests/Poems/PoemMatcherChainTests.cs` | Delete — replaced by chain tests |

### Files to Delete (empty scaffolding)
| File | Reason |
|------|--------|
| `Haiku.Services.Tests/Haiku/SyllableEngineTests.cs` | Empty stub |
| `Haiku.Services.Tests/Haiku/PoemEngineTests.cs` | Empty stub |
| `Haiku.Services.Tests/Haiku/PoemServiceTests.cs` | Empty stub |
| `Haiku.Services.Tests/Slices/Poems/DetectPoemTypeQueryHandlerTests.cs` | Empty stub (actual tests in Haiku.Tests) |
| `Haiku.Services.Tests/Slices/Poems/CreatePoemCommandHandlerTests.cs` | Empty stub |
| `Haiku.Services.Tests/Slices/Poems/DeletePoemCommandHandlerTests.cs` | Empty stub |
| `Haiku.Services.Tests/Slices/Poems/GetPoemQueryHandlerTests.cs` | Empty stub |
| `Haiku.Services.Tests/Slices/Moderation/HidePoemCommandHandlerTests.cs` | Empty stub |
| `Haiku.Services.Tests/Slices/Moderation/UnhidePoemCommandHandlerTests.cs` | Empty stub |
| `Haiku.Services.Tests/Slices/WordSearch/SearchPoemsByWordQueryHandlerTests.cs` | Empty stub |

---

## Phase 10: Cleanup

```bash
# Delete old folders
rm -rf src/Haiku.Services/Haiku/
rm -rf src/Haiku.Services/Poems/Matchers/

# Build verification
dotnet build Haiku.slnx

# Test verification
dotnet test

# Format
dotnet format style
dotnet format analyzers
dotnet csharpier format .
```

---

## Summary

| Metric | Count |
|--------|-------|
| New files (source) | ~25 |
| Deleted files | ~12 |
| Modified files | ~20 |
| New test files | ~20 |
| Updated test files | ~5 |
| Deleted test files | ~5 |
| Total PRD files updated | 2 |
| Build + test cycles | 3 (after Domain, after Services, after Web) |
