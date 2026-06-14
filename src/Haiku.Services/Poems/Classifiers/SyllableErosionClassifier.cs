using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Erosion form: per-line syllable counts decrease by exactly 1
/// each line, descending to 1. Minimum 3 lines.
/// </summary>
public sealed class SyllableErosionClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3600;

    /// <summary>
    /// Gets the type metadata for the Syllable Erosion form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the descending-count syllable pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableErosion,
            DisplayName: "Syllable Erosion",
            Description: "Syllable counts decrease by exactly 1 each line: n, n-1, ..., 1. Min 3 lines, last line = 1.",
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
        if (!PatternMatchers.IsErosion(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
