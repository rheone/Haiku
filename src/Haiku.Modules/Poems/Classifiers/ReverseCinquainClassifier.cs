using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the reverse cinquain form: exactly five lines with a 2-8-6-4-2 syllable pattern.
/// The mirror of the American cinquain's 2-4-6-8-2 progression, descending from 8 back to 2.
/// </summary>
public sealed class ReverseCinquainClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 1000;

    /// <summary>
    /// Gets the type metadata for the reverse cinquain form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the 2-8-6-4-2 syllable-based form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.ReverseCinquain,
            DisplayName: "Reverse Cinquain",
            Description: "A five-line poem with 2-8-6-4-2 syllable pattern, the reverse of the American cinquain.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [2, 8, 6, 4, 2],
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
        if (lines.Length != 5 || syllableCounts.Length != 5)
        {
            definition = null;
            return false;
        }

        if (
            syllableCounts[0] == 2
            && syllableCounts[1] == 8
            && syllableCounts[2] == 6
            && syllableCounts[3] == 4
            && syllableCounts[4] == 2
        )
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
