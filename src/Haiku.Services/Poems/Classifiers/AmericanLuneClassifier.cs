using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class AmericanLuneClassifier : IPoemClassifier
{
    public int Priority => 400;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.AmericanLune,
            DisplayName: "American Lune",
            Description: "A three-line modern American adaptation of haiku with 3-5-3 syllable pattern. Formerly called Minimalist.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [3, 5, 3],
            WordPattern: null
        );

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length != 3 || syllableCounts.Length != 3)
        {
            definition = null;
            return false;
        }

        if (syllableCounts[0] == 3 && syllableCounts[1] == 5 && syllableCounts[2] == 3)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
