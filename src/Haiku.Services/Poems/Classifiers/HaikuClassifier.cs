using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class HaikuClassifier : IPoemClassifier
{
    public int Priority => 200;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Haiku,
            DisplayName: "Haiku",
            Description: "A traditional Japanese form with 5-7-5 syllable pattern across three lines.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [5, 7, 5],
            WordPattern: null
        );

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        out PoemDefinition? definition
    )
    {
        if (lines.Length != 3 || syllableCounts.Length != 3)
        {
            definition = null;
            return false;
        }

        if (syllableCounts[0] == 5 && syllableCounts[1] == 7 && syllableCounts[2] == 5)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
