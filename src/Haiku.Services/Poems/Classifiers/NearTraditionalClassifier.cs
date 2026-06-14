using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class NearTraditionalClassifier : IPoemClassifier
{
    public int Priority => 700;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.NearTraditional,
            DisplayName: "Near-Traditional",
            Description: "A three-line nonstandard approximation of haiku with 4-6-4 syllable pattern.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [4, 6, 4],
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

        if (syllableCounts[0] == 4 && syllableCounts[1] == 6 && syllableCounts[2] == 4)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
