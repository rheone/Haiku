using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Kelly Lune form: exactly three lines with a 5-3-5 syllable pattern.
/// A modern American form created by poet Robert Kelly.
/// </summary>
public sealed class KellyLuneClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 500;

    /// <summary>
    /// Gets the type metadata for the Kelly Lune form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the 5-3-5 syllable-based modern American form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.KellyLune,
            DisplayName: "Kelly Lune",
            Description: "A three-line form created by Robert Kelly with 5-3-5 syllable pattern.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [5, 3, 5],
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
        if (lines.Length != 3 || syllableCounts.Length != 3)
        {
            definition = null;
            return false;
        }

        if (syllableCounts[0] == 5 && syllableCounts[1] == 3 && syllableCounts[2] == 5)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
