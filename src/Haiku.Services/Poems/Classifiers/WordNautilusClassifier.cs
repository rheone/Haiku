using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class WordNautilusClassifier : IPoemClassifier
{
    public int Priority => 4100;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordNautilus,
            DisplayName: "Word Nautilus",
            Description: "Word counts follow quadratic growth (constant second difference). Example: 2, 3, 5, 8, 12, ... Min 3 lines.",
            Category: PoemCategory.NonTraditional,
            Scaffold: PoemScaffold.WordBased,
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
        var wordCounts = tokenizedLines.Select(t => t.WordCount).ToArray();

        if (!PatternMatchers.IsNautilus(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
