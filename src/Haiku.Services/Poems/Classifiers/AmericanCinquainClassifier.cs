using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class AmericanCinquainClassifier : IPoemClassifier
{
    public int Priority => 900;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.AmericanCinquain,
            DisplayName: "American Cinquain",
            Description: "A five-line poem with 2-4-6-8-2 syllable pattern, invented by Adelaide Crapsey.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [2, 4, 6, 8, 2],
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
            && syllableCounts[1] == 4
            && syllableCounts[2] == 6
            && syllableCounts[3] == 8
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
