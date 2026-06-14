namespace Haiku.Domain.Enums;

// These 15 types are detected by PoemMatcherChain in Haiku.Services.
// Detection is primarily based on syllable pattern; Freeform must be user-specified
// since it has no fixed constraints.

/// <summary>
/// Identifies the structural form or poetic style of a poem.
/// </summary>
/// <remarks>
/// <para>Each value represents a distinct poetic form with specific syllable and line
/// constraints. Types are detected automatically during poem creation using a chain
/// of matchers in <c>PoemMatcherChain</c>. The Freeform type is an exception — it
/// carries no constraints and must be explicitly selected by the author. When a poem
/// matches multiple forms, the first matching form in priority order is assigned.</para>
/// </remarks>
public enum PoemType
{
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

    /// <summary>
    /// A poem with no fixed syllable constraints. Must be explicitly chosen by the poet.
    /// </summary>
    Freeform,
}
