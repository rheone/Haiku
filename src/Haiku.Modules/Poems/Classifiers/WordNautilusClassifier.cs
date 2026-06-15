using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Word Nautilus form: per-line word counts follow quadratic growth
/// with a constant second difference. Minimum 3 lines.
/// </summary>
public sealed class WordNautilusClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 4100;

    /// <summary>
    /// Gets the type metadata for the Word Nautilus form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the quadratic-growth word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordNautilus,
            DisplayName: "Word Nautilus",
            Description: "Word counts follow quadratic growth (constant second difference). Example: 2, 3, 5, 8, 12, ... Min 3 lines.",
            Category: PoemCategory.NonTraditional,
            Scaffold: PoemScaffold.WordBased,
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
        var wordCounts = tokenizedLines.Select(t => t.WordCount).ToArray();

        if (!PatternMatchers.IsNautilus(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
