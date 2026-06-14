using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class SyllableWaveClassifier : IPoemClassifier
{
    public int Priority => 2200;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableWave,
            DisplayName: "Syllable Wave",
            Description: "Syllable counts form a symmetric wave: n, n+1, ..., peak, ..., n+1, n. Min 5 lines, odd.",
            Category: PoemCategory.NonTraditional,
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
        if (!WaveClassifierBase.IsSingleWave(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
