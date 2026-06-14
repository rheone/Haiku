using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class ReverseCinquainClassifier : IPoemClassifier
{
    public int Priority => 1000;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.ReverseCinquain,
            DisplayName: "Reverse Cinquain",
            Description: "A five-line poem with 2-8-6-4-2 syllable pattern, the reverse of the American cinquain.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [2, 8, 6, 4, 2],
            WordPattern: null
        );

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length != 5 || syllableCounts.Length != 5)
        {
            definition = null;
            return false;
        }

        if (
            syllableCounts[0] == 2
            && syllableCounts[1] == 8
            && syllableCounts[2] == 6
            && syllableCounts[3] == 4
            && syllableCounts[4] == 2
        )
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
