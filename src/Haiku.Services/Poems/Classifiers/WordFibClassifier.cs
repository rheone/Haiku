using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Word Fib form: each line's word count follows the Fibonacci
/// sequence (1, 1, 2, 3, 5, 8, ...). Minimum 3 lines.
/// </summary>
public sealed class WordFibClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 1900;

    /// <summary>
    /// Gets the type metadata for the Word Fib form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the Fibonacci-sequence word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordFib,
            DisplayName: "Word Fib",
            Description: "Each line's word count follows the Fibonacci sequence (1, 1, 2, 3, 5, 8, ...). Min 3 lines.",
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

        if (!FibonacciSequence.IsFibMatch(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
