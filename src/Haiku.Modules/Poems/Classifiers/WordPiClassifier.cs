using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Word Pi form: each line's word count follows the decimal
/// digits of pi (3, 1, 4, 1, 5, 9, ...), skipping zero digits. Minimum 3 lines.
/// </summary>
public sealed class WordPiClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 1700;

    /// <summary>
    /// Gets the type metadata for the Word Pi form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the pi-digit-matching word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordPi,
            DisplayName: "Word Pi",
            Description: "Each line's word count follows the decimal digits of pi (3.14159...), skipping zeros. Min 3 lines.",
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

        if (!PiSequence.IsPiMatch(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
