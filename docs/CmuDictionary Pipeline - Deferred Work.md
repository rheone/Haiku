# CMU Dictionary Pipeline — Deferred Work

This document catalogs features and improvements that were identified during the CMU dictionary pipeline refactor but intentionally deferred to keep the initial implementation focused. Each item includes the motivation, current state, and recommended approach.

---

## 1. CustomDictionaryProvider (DB-Backed User Words)

**Motivation:** Users can submit words not in the CMU dictionary (proper nouns, slang, neologisms) with a custom syllable count. These live in the `CustomDictionaryWords` SQL table and must take priority over the CMU dictionary in the `ISyllableProvider` chain.

**Current state:** A stub `CustomDictionaryProvider` exists at `src/Haiku.Services/Syllables/Providers/CustomDictionaryProvider.cs`. It implements `ISyllableProvider` and is registered first in the chain. However, it has no database integration — it accepts a `Dictionary<string, int>` in its constructor, which must be populated externally.

**Recommended approach:**
- `CustomDictionaryProvider` should accept `IDictionaryRepository` (or a scoped factory) and query approved words at construction time, caching them in memory.
- Cache invalidation: when a word is approved or rejected, the singleton provider's cache must be refreshed. Options:
  - Event-based: publish a `DictionaryChangedEvent` via `MicroMediator`; the provider's handler re-queries the DB.
  - Time-based: periodic refresh (less ideal).
  - Hybrid: invalidate on publish, fall back to last-known-good on cache miss.
- The provider's in-memory cache should use `ConcurrentDictionary` for thread safety.

**Location:** `src/Haiku.Services/Syllables/Providers/CustomDictionaryProvider.cs`

---

## 2. PoemClassifierChain (Replacing Hardcoded Definitions)

**Motivation:** `PoemEngine` currently has a hardcoded `PoemDefinition` dictionary and its own `IsValidPoem`/`DetectPoemType` methods that duplicate logic in the `IPoemClassifier` chain. The classifier chain is already built but not yet the sole source of type detection truth.

**Current state:** `PoemClassifierChain` is operational. `PoemEngine.DetectPoemType` delegates to it when `_chain` is available. But `PoemEngine.GetDefinition()` and `PoemEngine.GetAllDefinitions()` still go through the chain (which is correct). The `PoemDefinition` static dictionary has been removed.

**Recommended approach:**
- Remove the `PoemEngine.IsValidPoem` and `PoemEngine.DetectPoemType` methods entirely — force all callers through the classifier chain or the appropriate query handler.
- Consolidate `DetectPoemTypeQueryHandler.StaticDetect` into the classifier chain or remove it in favor of always using the chain.

---

## 3. Homograph-Aware Syllable Resolution

**Motivation:** Words like "record" (noun 2 syllables vs verb 3 syllables) have multiple pronunciations with different syllable counts. Currently `CmuDictionaryProvider.TryCountSyllables` returns the **first** entry, which is correct for some contexts but wrong for others.

**Current state:** The JSON schema supports homographs via arrays. The provider returns `entries[0].SyllableCount`. A test verifies this behavior.

**Recommended approach:**
- Implement context-aware selection in `SyllableEngine`:
  - Part-of-speech tagging: analyze neighboring words to determine noun vs verb usage.
  - Frequency-based: return the most common pronunciation for the word's POS.
  - User override: allow poets to pick the intended pronunciation.
- This is a significant feature that needs UX design input.

**Status:** Schema-ready. Selection logic deferred.

---

## 4. Heuristic Algorithm Validation Report

**Motivation:** The `HeuristicSyllableProvider` is a fallback for words not in any dictionary. Its accuracy can be measured by running it against the CMU dictionary and comparing results.

**Current state:** The heuristic algorithm is well-documented in `HeuristicSyllableProvider.cs`. Known limitations include:
- Does not normalize diphthongs — adjacent vowel letters are always counted as one group (correct for most cases, overcounts some vowel sequences across syllable boundaries).
- Does not handle compound words or affixes as separate syllables.
- The consonant+le and silent-e rules are implemented as documented.

**Recommended approach:**
- Add a `--validate` flag to `tools/build-cmudict.cs` that:
  - Runs each CMU dictionary word through the heuristic.
  - Produces a report of mismatches (heuristic count vs CMU count) grouped by word pattern.
  - Calculates overall accuracy percentage.
- Use the report to refine the algorithm.
- This is a one-time analysis task, not recurring.

---

## 5. Diagnostic Mode (SC-08)

**Motivation:** Users want to see per-word syllable resolution tiers ("CMU", "Heuristic", "Custom", "Numeral", etc.) in the UI to understand why a word got a particular syllable count.

**Current state:** The tier information is available in `SyllableResult.Tier` and `LineSyllableResult`, but there's no UI to display it.

**Recommended approach:**
- Add a diagnostic toggle to the composer UI (hidden by default, enabled via user setting or URL parameter).
- When enabled, render each word with a small badge showing its syllable count and tier.
- The tier data flows naturally through `LineSyllableResult.Words[].Tier`.

---

## 6. build-cmudict.cs Enhancements

**Motivation:** The current build tool is functional but has room for improvement.

**Recommended approach:**
- Pin to a specific upstream commit (currently uses `master` branch).
- Validate output against the `CmuDictionaryProvider` schema before writing.
- Add a `--validate` flag to run heuristic accuracy analysis (see §4 above).
- Add progress reporting for large downloads.
- Integrate into CI: run as part of a scheduled workflow to detect upstream changes.

---

## 7. Remove PoemEngine VowelGroupCount Fallback

**Motivation:** `PoemEngine` has a `VowelGroupCountFallback` method used when `_syllableEngine` is null. Once all code paths use the injected `SyllableEngine`, this fallback can be removed.

**Current state:** The fallback exists for backward compatibility when PoemEngine is constructed without injected services (primarily in tests).

**Recommended approach:**
- Update all test and production code to inject `SyllableEngine`.
- Remove the fallback methods: `VowelGroupCountFallback`, `AnalyzeLineFallback`, `GetLastWordFallback`.
- Make `SyllableEngine`, `CmuDictionaryProvider`, and `RhymingEngine` required constructor parameters (remove `= null` defaults).

---

## Priority Order

| Item | Impact | Effort | Suggested Phase |
|------|--------|--------|-----------------|
| 1. CustomDictionaryProvider | High (user feature) | Medium | Next sprint |
| 2. Classifier chain consolidation | Medium (cleanup) | Small | Next cleanup pass |
| 3. Homograph resolution | Medium (accuracy) | Large | After UX design |
| 4. Heuristic validation | Low (tooling) | Small | Before next heuristic change |
| 5. Diagnostic mode | Medium (UX) | Medium | Per product priority |
| 6. Build tool enhancements | Low (tooling) | Small | As needed |
| 7. Remove fallbacks | Low (cleanup) | Small | After all consumers updated |
