using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Word Reverse Fib form: each line's word count follows the
/// reversed prefix of the Fibonacci sequence. Minimum 3 lines.
/// </summary>
public sealed class WordReverseFibClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 2100;

    /// <summary>
    /// Gets the type metadata for the Word Reverse Fib form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the reverse-Fibonacci word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordReverseFib,
            DisplayName: "Word Reverse Fib",
            Description: "Each line's word count follows the reversed prefix of the Fibonacci sequence. Min 3 lines.",
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

        if (!FibonacciSequence.IsReverseFibMatch(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
