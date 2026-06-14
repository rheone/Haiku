using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class SyllableReverseFibClassifier : IPoemClassifier
{
    public int Priority => 2000;

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
