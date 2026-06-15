namespace Haiku.Modules.Shared.Domain.Enums;

// These types are detected by PoemClassifierChain in Haiku.Services.
// Detection is primarily based on syllable/word pattern; Freeform must be user-specified
// since it has no fixed constraints.
//
// ╔══════════════════════════════════════════════════════════════════════════╗
// ║  KEEP THIS ENUM IN SYNC WITH THE CLASSIFIER TYPEIDS                    ║
// ║                                                                        ║
// ║  The PoemType enum IS the single source of truth for type identity.     ║
// ║  Classifiers pass the enum value to PoemTypeInfo, and the TypeId       ║
// ║  string is DERIVED AUTOMATICALLY via kebab-case conversion:            ║
// ║    SyllableCrestWave  →  "syllable-crest-wave"                         ║
// ║  No manual TypeId string exists anywhere in the system.                ║
// ║                                                                        ║
// ║  To add a new poem type:                                               ║
// ║    1. Add a value here (PascalCase)                                    ║
// ║    2. Create a classifier passing PoemType.YourNewValue                 ║
// ║                                                                        ║
// ║  To remove a poem type:                                                ║
// ║    1. Remove the classifier                                            ║
// ║    2. Optionally remove the enum value (safe to keep for historic data) ║
// ║                                                                        ║
// ║  If an enum value has no matching classifier, detection still works     ║
// ║  but the type will never be auto-detected.                             ║
// ╚══════════════════════════════════════════════════════════════════════════╝

/// <summary>
/// Identifies the structural form or poetic style of a poem.
/// </summary>
/// <remarks>
/// <para>Each value represents a distinct poetic form with specific structural
/// constraints. Types are detected automatically during poem creation using a chain
/// of classifiers in <c>PoemClassifierChain</c>. The Freeform type is an exception — it
/// carries no constraints and must be explicitly selected by the author. When a poem
/// matches multiple forms, the first matching form in priority order is assigned.</para>
/// <para><b>Adding a new type:</b> Add the enum value here (PascalCase), then create an
/// <c>IPoemClassifier</c> implementation passing this enum value to <c>PoemTypeInfo</c>.
/// The <c>TypeId</c> string is derived automatically via kebab-case conversion —
/// no manual string-to-enum mapping exists anywhere in the system.</para>
/// </remarks>
public enum PoemType
{
    // =====================================================================
    // Traditional forms (Heritage: Traditional, Scaffold: SyllableBased)
    // =====================================================================

    /// <summary>
    /// A traditional Japanese form with 5-7-5 syllable pattern across three lines.
    /// </summary>
    Haiku,

    /// <summary>
    /// A single-line poem where total syllables must be between 4 and 17 inclusive.
    /// </summary>
    Monoku,

    /// <summary>
    /// A five-line Japanese form with 5-7-5-7-7 syllable pattern.
    /// </summary>
    Tanka,

    /// <summary>
    /// A three-line classical Japanese form with 5-7-7 syllable pattern.
    /// </summary>
    Katauta,

    /// <summary>
    /// A six-line poem equivalent to two joined katauta (5-7-7-5-7-7).
    /// </summary>
    Sedoka,

    /// <summary>
    /// A long poem with alternating 5-7 syllable lines, ending with 5-7-7. Always an odd number of lines.
    /// </summary>
    Choka,

    /// <summary>
    /// A three-line modern American adaptation of haiku with 3-5-3 syllable pattern. Formerly called Minimalist.
    /// </summary>
    AmericanLune,

    /// <summary>
    /// A three-line form created by Robert Kelly with 5-3-5 syllable pattern.
    /// </summary>
    KellyLune,

    /// <summary>
    /// A five-line poem with 2-4-6-8-2 syllable pattern, invented by Adelaide Crapsey.
    /// </summary>
    AmericanCinquain,

    /// <summary>
    /// A five-line poem with 2-8-6-4-2 syllable pattern, the reverse of the American cinquain.
    /// </summary>
    ReverseCinquain,

    /// <summary>
    /// A ten-line poem formed by concatenating an American cinquain and a Reverse cinquain (2-4-6-8-2-2-8-6-4-2).
    /// </summary>
    MirrorCinquain,

    /// <summary>
    /// A nine-line poem formed by merging an American cinquain with its reverse, dropping the center line (2-4-6-8-2-8-6-4-2).
    /// </summary>
    ButterflyCinquain,

    /// <summary>
    /// A poem where every line has the same syllable count. Any number of lines n >= 2.
    /// </summary>
    Isosyllabic,

    /// <summary>
    /// A three-line nonstandard haiku-inspired ultra-short form with 2-3-2 syllable pattern.
    /// </summary>
    Compressed,

    /// <summary>
    /// A three-line nonstandard approximation of haiku with 4-6-4 syllable pattern.
    /// </summary>
    NearTraditional,

    // =====================================================================
    // Non-traditional — Sequence-based (Heritage: NonTraditional)
    // =====================================================================

    /// <summary>
    /// Syllable counts follow the decimal digits of pi (starting after the decimal point, skipping zeros).
    /// Min 3 lines.
    /// </summary>
    SyllablePi,

    /// <summary>
    /// Word counts follow the decimal digits of pi (starting after the decimal point, skipping zeros).
    /// Min 3 lines.
    /// </summary>
    WordPi,

    /// <summary>
    /// Syllable counts follow the Fibonacci sequence (1, 1, 2, 3, 5, 8, ...). Min 3 lines.
    /// </summary>
    SyllableFib,

    /// <summary>
    /// Word counts follow the Fibonacci sequence (1, 1, 2, 3, 5, 8, ...). Min 3 lines.
    /// </summary>
    WordFib,

    /// <summary>
    /// Syllable counts follow the reversed prefix of the Fibonacci sequence. Min 3 lines.
    /// </summary>
    SyllableReverseFib,

    /// <summary>
    /// Word counts follow the reversed prefix of the Fibonacci sequence. Min 3 lines.
    /// </summary>
    WordReverseFib,

    // =====================================================================
    // Non-traditional — Wave family (Heritage: NonTraditional)
    // =====================================================================

    /// <summary>
    /// Syllable counts form a symmetric wave: n, n+1, ..., peak, ..., n+1, n. Min 5 lines.
    /// </summary>
    SyllableWave,

    /// <summary>
    /// Word counts form a symmetric wave: n, n+1, ..., peak, ..., n+1, n. Min 5 lines.
    /// </summary>
    WordWave,

    /// <summary>
    /// Two or more syllable waves chained, each with a smaller peak than the previous. Same shape.
    /// </summary>
    SyllableCrestWave,

    /// <summary>
    /// Two or more word waves chained, each with a smaller peak than the previous. Same shape.
    /// </summary>
    WordCrestWave,

    /// <summary>
    /// Two or more syllable waves chained, each with a larger peak than the previous. Same shape.
    /// </summary>
    SyllableCrashWave,

    /// <summary>
    /// Two or more word waves chained, each with a larger peak than the previous. Same shape.
    /// </summary>
    WordCrashWave,

    // =====================================================================
    // Non-traditional — Constraint-based (Heritage: NonTraditional)
    // =====================================================================

    /// <summary>
    /// Each line's syllable count is a prime number. Min 3 lines.
    /// </summary>
    SyllablePrime,

    /// <summary>
    /// Each line's word count is a prime number. Min 3 lines.
    /// </summary>
    WordPrime,

    /// <summary>
    /// Syllable counts alternate between two distinct values (a, b, a, b, ...). Min 4 lines, even count.
    /// </summary>
    SyllablePulse,

    /// <summary>
    /// Word counts alternate between two distinct values (a, b, a, b, ...). Min 4 lines, even count.
    /// </summary>
    WordPulse,

    /// <summary>
    /// Syllable counts follow the Collatz (hailstone) sequence from n down to 1. Min 3 lines.
    /// </summary>
    SyllableHailstone,

    /// <summary>
    /// Word counts follow the Collatz (hailstone) sequence from n down to 1. Min 3 lines.
    /// </summary>
    WordHailstone,

    /// <summary>
    /// Syllable counts increase by exactly 1 each line: n, n+1, n+2, ..., n+k. Min 3 lines.
    /// </summary>
    SyllableStair,

    /// <summary>
    /// Word counts increase by exactly 1 each line: n, n+1, n+2, ..., n+k. Min 3 lines.
    /// </summary>
    WordStair,

    /// <summary>
    /// Syllable counts decrease by exactly 1 each line: n, n-1, ..., 1. Min 3 lines. Last line = 1.
    /// </summary>
    SyllableErosion,

    /// <summary>
    /// Word counts decrease by exactly 1 each line: n, n-1, ..., 1. Min 3 lines. Last line = 1.
    /// </summary>
    WordErosion,

    /// <summary>
    /// Syllable counts increase by exactly 1 each line starting from 1: 1, 2, 3, ..., n. Min 3 lines.
    /// </summary>
    SyllableMountain,

    /// <summary>
    /// Word counts increase by exactly 1 each line starting from 1: 1, 2, 3, ..., n. Min 3 lines.
    /// </summary>
    WordMountain,

    /// <summary>
    /// Syllable counts follow quadratic growth (constant second difference): a, b, c, ... where each
    /// step increase grows by 1. Example: 2, 3, 5, 8, 12, 17, ... Min 3 lines.
    /// </summary>
    SyllableNautilus,

    /// <summary>
    /// Word counts follow quadratic growth (constant second difference). Min 3 lines.
    /// </summary>
    WordNautilus,

    // =====================================================================
    // Catch-all
    // =====================================================================

    /// <summary>
    /// A poem with no fixed structural constraints. Must be explicitly chosen by the poet.
    /// </summary>
    Freeform,
}
