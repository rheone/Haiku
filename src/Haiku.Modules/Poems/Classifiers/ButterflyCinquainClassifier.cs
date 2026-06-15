using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the butterfly cinquain form: exactly nine lines with a 2-4-6-8-2-8-6-4-2 syllable pattern.
/// Formed by merging an American cinquain with its reverse, omitting the duplicated center line.
/// </summary>
public sealed class ButterflyCinquainClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 1200;

    /// <summary>
    /// Gets the type metadata for the butterfly cinquain form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the 2-4-6-8-2-8-6-4-2 syllable-based form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.ButterflyCinquain,
            DisplayName: "Butterfly Cinquain",
            Description: "A nine-line poem formed by merging an American cinquain with its reverse, dropping the center line (2-4-6-8-2-8-6-4-2).",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [2, 4, 6, 8, 2, 8, 6, 4, 2],
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
        if (lines.Length != 9 || syllableCounts.Length != 9)
        {
            definition = null;
            return false;
        }

        if (
            syllableCounts[0] == 2
            && syllableCounts[1] == 4
            && syllableCounts[2] == 6
            && syllableCounts[3] == 8
            && syllableCounts[4] == 2
            && syllableCounts[5] == 8
            && syllableCounts[6] == 6
            && syllableCounts[7] == 4
            && syllableCounts[8] == 2
        )
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
