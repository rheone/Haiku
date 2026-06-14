using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class ButterflyCinquainClassifier : IPoemClassifier
{
    public int Priority => 1200;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.ButterflyCinquain,
            DisplayName: "Butterfly Cinquain",
            Description: "A nine-line poem formed by merging an American cinquain with its reverse, dropping the center line (2-4-6-8-2-8-6-4-2).",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [2, 4, 6, 8, 2, 8, 6, 4, 2],
            WordPattern: null
        );

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length != 9 || syllableCounts.Length != 9)
        {
            definition = null;
            return false;
        }

        if (
            syllableCounts[0] == 2
            && syllableCounts[1] == 4
            && syllableCounts[2] == 6
            && syllableCounts[3] == 8
            && syllableCounts[4] == 2
            && syllableCounts[5] == 8
            && syllableCounts[6] == 6
            && syllableCounts[7] == 4
            && syllableCounts[8] == 2
        )
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
