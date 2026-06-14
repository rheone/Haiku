using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class KatautaClassifier : IPoemClassifier
{
    public int Priority => 300;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Katauta,
            DisplayName: "Katauta",
            Description: "A three-line classical Japanese form with 5-7-7 syllable pattern.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [5, 7, 7],
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

        if (syllableCounts[0] == 5 && syllableCounts[1] == 7 && syllableCounts[2] == 7)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
