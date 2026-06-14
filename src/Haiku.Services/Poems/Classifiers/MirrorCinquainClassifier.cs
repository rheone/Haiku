using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class MirrorCinquainClassifier : IPoemClassifier
{
    public int Priority => 1300;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.MirrorCinquain,
            DisplayName: "Mirror Cinquain",
            Description: "A ten-line poem formed by concatenating an American cinquain and a Reverse cinquain (2-4-6-8-2-2-8-6-4-2).",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [2, 4, 6, 8, 2, 2, 8, 6, 4, 2],
            WordPattern: null
        );

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length != 10 || syllableCounts.Length != 10)
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
            && syllableCounts[5] == 2
            && syllableCounts[6] == 8
            && syllableCounts[7] == 6
            && syllableCounts[8] == 4
            && syllableCounts[9] == 2
        )
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
