using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class MonokuClassifier : IPoemClassifier
{
    public int Priority => 100;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Monoku,
            DisplayName: "Monoku",
            Description: "A single-line poem where total syllables must be between 4 and 17 inclusive.",
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
        var totalSyllables = syllableCounts.Sum();

        if (lines.Length == 1 && totalSyllables >= 4 && totalSyllables <= 17)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
