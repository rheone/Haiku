using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class SyllableFibClassifier : IPoemClassifier
{
    public int Priority => 1800;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableFib,
            DisplayName: "Syllable Fib",
            Description: "Each line's syllable count follows the Fibonacci sequence (1, 1, 2, 3, 5, 8, ...). Min 3 lines.",
            Category: PoemCategory.NonTraditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: null,
            WordPattern: null
        );

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (!FibonacciSequence.IsFibMatch(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
