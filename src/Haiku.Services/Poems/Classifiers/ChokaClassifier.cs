using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class ChokaClassifier : IPoemClassifier
{
    public int Priority => 1400;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Choka,
            DisplayName: "Choka",
            Description: "A long poem with alternating 5-7 syllable lines, ending with 5-7-7. Always an odd number of lines.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: null,
            WordPattern: null
        );

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        var n = lines.Length;

        if (n < 7 || n % 2 != 1)
        {
            definition = null;
            return false;
        }

        // Alternating 5-7 for all lines except the last three which form 5-7-7
        for (var i = 0; i <= n - 4; i++)
        {
            if (syllableCounts[i] != (i % 2 == 0 ? 5 : 7))
            {
                definition = null;
                return false;
            }
        }

        if (syllableCounts[n - 3] != 5 || syllableCounts[n - 2] != 7 || syllableCounts[n - 1] != 7)
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
