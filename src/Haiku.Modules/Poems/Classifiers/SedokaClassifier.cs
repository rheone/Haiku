using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the sedoka form: exactly six lines with a 5-7-7-5-7-7 syllable pattern.
/// Structurally equivalent to two joined katauta stanzas.
/// </summary>
public sealed class SedokaClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 1100;

    /// <summary>
    /// Gets the type metadata for the sedoka form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the 5-7-7-5-7-7 syllable-based traditional form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Sedoka,
            DisplayName: "Sedoka",
            Description: "A six-line poem equivalent to two joined katauta (5-7-7-5-7-7).",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [5, 7, 7, 5, 7, 7],
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
        if (lines.Length != 6 || syllableCounts.Length != 6)
        {
            definition = null;
            return false;
        }

        if (
            syllableCounts[0] == 5
            && syllableCounts[1] == 7
            && syllableCounts[2] == 7
            && syllableCounts[3] == 5
            && syllableCounts[4] == 7
            && syllableCounts[5] == 7
        )
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
