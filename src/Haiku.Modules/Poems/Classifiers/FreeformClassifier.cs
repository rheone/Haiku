namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Catch-all classifier that matches any poem with no fixed syllable or word constraints.
/// Always succeeds as the final fallback in the classifier chain.
/// </summary>
public sealed class FreeformClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => int.MaxValue;

    /// <summary>
    /// Gets the type metadata for the freeform (unrestricted) poem type.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the catch-all unrestricted form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Freeform,
            DisplayName: "Freeform",
            Description: "A poem with no fixed syllable constraints.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: null,
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
        definition = ClassifierBuilder.Build(this);

        return true;
    }
}
