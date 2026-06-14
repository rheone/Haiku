# Potential Poem Types

This document catalogs poem type ideas that were considered but not implemented in the
first pass. Each entry includes the concept, algorithm, detection approach, and the
reason it was deferred. These are well-defined enough that a developer could implement
them by creating a `PatternMatchers` method + a thin classifier pair.

---

## 1. SyllableStepdown / WordStepdown

**Concept:** Each line has exactly one fewer word/syllable than the previous, until
reaching the terminal value. Mirrors `Mountain` (ascending) but descending.

```
n, n-1, n-2, ..., n-k
```

**Status:** Deferred — effectively identical to `Erosion` but without the requirement
that the last line equals 1. Erosion is the stricter version and was preferred.

**Detection algorithm:**
```
if len < 3: return false
for i in range(1, len):
    if counts[i] != counts[i-1] - 1: return false
return true
```

---

## 2. SyllableStopAndGo / WordStopAndGo

**Concept:** Alternates between a fixed value and growing values: a, a+1, a, a+2, a, a+3, ...
Creates a hiccup rhythm — a single short line inserted between progressively swelling lines.

**Status:** Deferred — quirky but hard to describe succinctly to users. Low poetic value.

**Detection algorithm:**
```
if len < 5: return false
if len % 2 == 0: return false  // must be odd
a = counts[0]
for i in range(1, len):
    if i % 2 == 1:  // odd index → growing
        if counts[i] != a + (i+1)/2: return false
    else:  // even index → fixed
        if counts[i] != a: return false
return true
```

---

## 3. SyllableCrescendo / WordCrescendo

**Concept:** A poem where each line is the same length _or longer_, and the total
increase over the whole poem is at least half the lines. Formalizes the "growing poem"
vibe without strict arithmetic constraints.

**Status:** Deferred — too vague for deterministic classification. Could match many
poems that aren't intentionally written this way (false positive risk).

**Detection algorithm:**
```
if len < 3: return false
increases = 0
for i in range(1, len):
    if counts[i] > counts[i-1]: increases++
return increases >= len / 2  // at least half the transitions increase
```

---

## 4. SyllableDiminuendo / WordDiminuendo

**Concept:** Mirror of Crescendo — each line is the same length or shorter, and at
least half the transitions are decreases.

**Status:** Deferred (same reason as Crescendo).

---

## 5. SyllablePalindrome / WordPalindrome

**Concept:** The sequence of counts is a palindrome: 2, 4, 6, 6, 4, 2.
The poem reads the same "shape" forward and backward. Symmetrical like a wave
but without requiring a +1 step pattern.

**Status:** Deferred — Wave already captures the most natural symmetric form.
Palindrome is a superset (all waves are palindromes, but not vice versa).

**Detection algorithm:**
```
if len < 3: return false
for i in range(len // 2):
    if counts[i] != counts[len-1-i]: return false
return true
```

---

## 6. SyllableTwist / WordTwist

**Concept:** The sequence increases by 1 for a while, then decreases by 1 for a while,
but not symmetrically. The peak doesn't have to be in the center. This is an
"asymmetric wave."

**Status:** Deferred — harder to detect (where does the peak end?). The Wave family
covers the symmetric case; an asymmetric variant would need a tolerance parameter,
increasing false positive risk.

**Detection algorithm (strict):**
```
if len < 4: return false
// Find peak = point where direction changes from up to down
peak_idx = -1
for i in range(1, len):
    if counts[i] < counts[i-1]:
        peak_idx = i - 1
        break
if peak_idx < 0 or peak_idx >= len - 1: return false
// Before peak: must increase by 1 each step
for i in range(1, peak_idx + 1):
    if counts[i] != counts[i-1] + 1: return false
// After peak: must decrease by 1 each step
for i in range(peak_idx + 1, len):
    if counts[i] != counts[i-1] - 1: return false
return true
```

---

## 7. SyllablePower / WordPower

**Concept:** Line counts follow powers of 2: 1, 2, 4, 8, 16, ...
Only works for a few lines before values become unreachable.

**Status:** Deferred — Fib is more natural (occurs in nature), powers of 2 grow
too fast (2^10 = 1024, impossible in 500 chars).

**Detection algorithm:**
```
if len < 3: return false
for i in range(len):
    if counts[i] != 2**i: return false
return true
```

---

## 8. SyllableFactorial / WordFactorial

**Concept:** Line counts follow factorial: 1, 2, 6, 24, 120, ...
Grows catastrophically fast. Only 2-3 lines are feasible.

**Status:** Deferred — too few viable lines. 4! = 24 syllables is ~8 words, barely
usable. 5! = 120 is impossible in 500 chars.

---

## 9. SyllableBell / WordBell

**Concept:** Two pulses joined: a, b, a, b, a, a, b, a, b, a. A single "bell" shape
where a pulse pattern has a longer sustain at the center.

**Status:** Deferred — Pulse already exists; Bell is just Pulse with a doubled center.
If we add Pulse, a separate Bell classifier seems excessive.

---

## 10. SyllableCountdown / WordCountdown

**Concept:** Each line counts down by 1 from a starting value, and the number of
lines equals the starting value. So n=5 gives 5, 4, 3, 2, 1 (5 lines).

**Status:** Deferred — Erosion achieves the same effect but doesn't require
`counts[0] == len`. Countdown is simply Erosion with an additional constraint.
A poet writing an erosion naturally tends toward this form anyway.

**Detection algorithm:**
```
if len < 3: return false
if counts[0] != len: return false
for i in range(len):
    if counts[i] != len - i: return false
return true
```

---

## 11. SyllablePyramid / WordPyramid

**Concept:** A wave followed by another wave with the same peak but wider base:
n, ..., peak, ..., n, n-1, ..., peak, ..., n-1. Like standing two waves side by side
with overlapping bases. Complex shape.

**Status:** Deferred — hard to describe and detect without false positives. The
Wave family covers all symmetric cases.

---

## 12. SyllableEcho / WordEcho

**Concept:** Each line is repeated once: a, a, b, b, c, c, ...
Even number of lines, each pair equal.

**Status:** Deferred — Pulse (a,b,a,b) is more interesting. Echo is Pulse where
a=b, which doesn't make sense. A separate Echo classifier would require
`counts[0]==counts[1]`, `counts[2]==counts[3]`, etc.

**Detection algorithm:**
```
if len < 4 or len % 2 != 0: return false
for i in range(0, len, 2):
    if counts[i] != counts[i+1]: return false
return true
```

---

## 13. SyllableAccelerando / WordAccelerando

**Concept:** Each step increase is larger than the last. Like Nautilus but without
a fixed second difference — just "each gap is bigger than the previous gap."
2, 5, 9, 14, 20 (gaps 3, 4, 5, 6).

**Status:** Deferred — Nautilus (constant second difference) is a stricter,
more elegant form of the same idea. Accelerando without a specific rule is
too vague for reliable detection.

---

## 14. Boolean / Binary Poem

**Concept:** Each line has either 1 or 2 syllables (or words). A binary sequence.
Creates staccato rhythms. The sequence could be user-chosen rather than
following a mathematical rule.

**Status:** Deferred — too permissive. Almost any poem with short lines could
accidentally match. Would generate excessive false positives.

---

## Design Notes

### Adding a new pattern type

1. Add a static detection method to `PatternMatchers.cs` in `SequenceHelpers/`
2. Create two thin classifier files (`SyllableXxxClassifier.cs`, `WordXxxClassifier.cs`)
   in `Classifiers/` directory, delegating to the new matcher
3. Add enum values to `PoemType.cs`
4. Add definitions to `PoemEngine.Definitions` dictionary
5. Register in `Program.cs` DI
6. Write tests using `ClassifierTestHelpers`

### Priority convention for new types

| Range   | Category                |
|---------|-------------------------|
| 100–1500| Traditional forms       |
| 1600–2100| Sequence-based (Pi, Fib) |
| 2200–2700| Wave family             |
| 2800–4100| Constraint-based (Prime, Pulse, etc.) |
| MAX     | Freeform                |
