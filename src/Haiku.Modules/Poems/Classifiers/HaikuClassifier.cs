namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the traditional haiku form: exactly three lines with a 5-7-5 syllable pattern.
/// The highest-priority form in the traditional Japanese category.
/// </summary>
public sealed class HaikuClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 200;

    /// <summary>
    /// Gets the type metadata for the haiku form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the 5-7-5 syllable-based traditional form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Haiku,
            DisplayName: "Haiku",
            Description: "A traditional Japanese form with 5-7-5 syllable pattern across three lines.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [5, 7, 5],
            WordPattern: null
        );

    /// <inheritdoc/>
    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length != 3 || syllableCounts.Length != 3)
        {
            definition = null;
            return false;
        }

        if (syllableCounts[0] == 5 && syllableCounts[1] == 7 && syllableCounts[2] == 5)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
