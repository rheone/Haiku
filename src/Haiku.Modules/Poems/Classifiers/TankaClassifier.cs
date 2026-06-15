using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the tanka form: exactly five lines with a 5-7-5-7-7 syllable pattern.
/// A classical Japanese form extending the haiku with two additional 7-syllable lines.
/// </summary>
public sealed class TankaClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 800;

    /// <summary>
    /// Gets the type metadata for the tanka form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the 5-7-5-7-7 syllable-based traditional form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Tanka,
            DisplayName: "Tanka",
            Description: "A five-line Japanese form with 5-7-5-7-7 syllable pattern.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [5, 7, 5, 7, 7],
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
            syllableCounts[0] == 5
            && syllableCounts[1] == 7
            && syllableCounts[2] == 5
            && syllableCounts[3] == 7
            && syllableCounts[4] == 7
        )
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
