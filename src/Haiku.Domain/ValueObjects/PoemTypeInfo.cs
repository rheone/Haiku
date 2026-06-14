using Haiku.Domain.Enums;

namespace Haiku.Domain.ValueObjects;

/// <summary>
/// Static metadata describing a poem type. Returned by <c>IPoemClassifier.Info</c>
/// and consumed by <c>ClassifierBuilder.Build()</c> to populate <see cref="PoemDefinition"/>.
/// </summary>
/// <param name="PoemType">The <see cref="Enums.PoemType"/> enum value (single source of truth).</param>
/// <param name="DisplayName">Human-readable name, e.g. "Syllable Pi".</param>
/// <param name="Description">Structural description for display to users.</param>
/// <param name="Category">Cultural lineage category.</param>
/// <param name="Scaffold">Measurement axis for constraints.</param>
/// <param name="SyllablePattern">Fixed per-line syllable pattern, or <c>null</c> for sequence/word types.</param>
/// <param name="WordPattern">Fixed per-line word pattern, or <c>null</c> for sequence/syllable types. Obsolete — all word-based types use dynamic sequence generation via <c>PatternGenerator</c>.</param>
/// <remarks>
/// <para><b>TypeId</b> is derived automatically from <see cref="PoemType"/> via kebab-case
/// conversion (e.g. <c>SyllableCrestWave</c> → <c>"syllable-crest-wave"</c>). Classifiers
/// must pass the enum value, not a manual string — this guarantees the TypeId is always
/// in sync with the enum.</para>
/// </remarks>
public record PoemTypeInfo(
    PoemType PoemType,
    string DisplayName,
    string Description,
    PoemCategory Category,
    PoemScaffold Scaffold,
    int[]? SyllablePattern,
    [property: Obsolete("Unused. All word-based types use dynamic sequence generation via PatternGenerator. Consider removal.")]
        int[]? WordPattern
)
{
    /// <summary>
    /// Unique stable identifier derived from <see cref="PoemType"/> via kebab-case convention.
    /// Example: <c>PoemType.SyllableCrestWave</c> → <c>"syllable-crest-wave"</c>.
    /// </summary>
    public string TypeId => PoemType.ToKebabCase();
}
