using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class IsosyllabicClassifier : IPoemClassifier
{
    public int Priority => 1500;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Isosyllabic,
            DisplayName: "Isosyllabic",
            Description: "A poem where every line has the same syllable count. Any number of lines n >= 2.",
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
        if (lines.Length < 2)
        {
            definition = null;
            return false;
        }

        var first = syllableCounts[0];
        if (first <= 0)
        {
            definition = null;
            return false;
        }

        for (var i = 1; i < syllableCounts.Length; i++)
        {
            if (syllableCounts[i] != first)
            {
                definition = null;
                return false;
            }
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
