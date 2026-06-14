using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class FreeformClassifier : IPoemClassifier
{
    public int Priority => int.MaxValue;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Freeform,
            DisplayName: "Freeform",
            Description: "A poem with no fixed syllable constraints.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: null,
            WordPattern: null
        );

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        out PoemDefinition? definition
    )
    {
        definition = ClassifierBuilder.Build(this);

        return true;
    }
}
