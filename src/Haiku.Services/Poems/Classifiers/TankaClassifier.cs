using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class TankaClassifier : IPoemClassifier
{
    public int Priority => 800;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Tanka,
            DisplayName: "Tanka",
            Description: "A five-line Japanese form with 5-7-5-7-7 syllable pattern.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [5, 7, 5, 7, 7],
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
            syllableCounts[0] == 5
            && syllableCounts[1] == 7
            && syllableCounts[2] == 5
            && syllableCounts[3] == 7
            && syllableCounts[4] == 7
        )
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
