using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Reverse Fib form: each line's syllable count follows the
/// reversed prefix of the Fibonacci sequence. Minimum 3 lines.
/// </summary>
public sealed class SyllableReverseFibClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 2000;

    /// <summary>
    /// Gets the type metadata for the Syllable Reverse Fib form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the reverse-Fibonacci syllable pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableReverseFib,
            DisplayName: "Syllable Reverse Fib",
            Description: "Each line's syllable count follows the reversed prefix of the Fibonacci sequence. Min 3 lines.",
            Category: PoemCategory.NonTraditional,
            Scaffold: PoemScaffold.SyllableBased,
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
        if (!FibonacciSequence.IsReverseFibMatch(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
