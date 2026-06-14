using Haiku.Domain.Enums;

namespace Haiku.Domain.ValueObjects;

/// <summary>
/// The result of classifying a poem, carrying both identity and display metadata.
/// Populated by <c>ClassifierBuilder</c> from the matching classifier's properties.
/// </summary>
/// <remarks>
/// <para>Metadata fields (<see cref="TypeId"/>, <see cref="DisplayName"/>, etc.) come
/// directly from the classifier that matched — no external registry needed.</para>
/// </remarks>
public record PoemDefinition
{
    /// <summary>Unique stable identifier from the matching classifier, e.g. "haiku", "syllable-pi".</summary>
    public string TypeId { get; init; } = string.Empty;

    /// <summary>Human-readable type name, e.g. "Syllable Pi".</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>Description of the type's structural rules.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Cultural lineage category.</summary>
    public PoemCategory Category { get; init; }

    /// <summary>Measurement axis for structural constraints.</summary>
    public PoemScaffold Scaffold { get; init; }

    /// <summary>
    /// Fixed syllable pattern per line, or <c>null</c> for sequence-based types.
    /// </summary>
    public int[]? SyllablePattern { get; init; }

    /// <summary>
    /// Fixed word-count pattern per line, or <c>null</c> for sequence-based types.
    /// Obsolete — all word-based types use dynamic sequence generation via <c>PatternGenerator</c>.
    /// </summary>
    [Obsolete("Unused. All word-based types use dynamic sequence generation via PatternGenerator. Consider removal.")]
    public int[]? WordPattern { get; init; }

    /// <summary>
    /// Backward-compatible enum value for DB storage.
    /// For traditional types this matches the enum; for new non-traditional types
    /// this returns <c>PoemType.Freeform</c> until the DB column is migrated to string.
    /// </summary>
    public PoemType Type { get; init; }

    /// <summary>
    /// Backward-compatible display name (same as <see cref="DisplayName"/>).
    /// </summary>
    public string Name { get; init; } = string.Empty;
}
