using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class SyllableCrashWaveClassifier : IPoemClassifier
{
    public int Priority => 2600;

    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableCrashWave,
            DisplayName: "Syllable Crash Wave",
            Description: "Two or more syllable waves chained, each with a larger peak. Same shape. Min 10 lines.",
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
        if (!WaveClassifierBase.IsCrashWave(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
