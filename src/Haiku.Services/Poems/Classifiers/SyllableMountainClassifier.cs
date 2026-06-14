using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Mountain form: per-line syllable counts start at 1 and
/// increase by exactly 1 each line. Minimum 3 lines.
/// </summary>
public sealed class SyllableMountainClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3800;

    /// <summary>
    /// Gets the type metadata for the Syllable Mountain form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the sequential-counting syllable pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableMountain,
            DisplayName: "Syllable Mountain",
            Description: "Syllable counts increase by exactly 1 starting from 1: 1, 2, 3, ..., n. Min 3 lines.",
            Category: PoemCategory.NonTraditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: null,
            WordPattern: null
        );

    /// <inheritdoc/>
    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (!PatternMatchers.IsMountain(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
