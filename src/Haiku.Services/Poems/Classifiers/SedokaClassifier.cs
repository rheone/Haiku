using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class SedokaClassifier : IPoemClassifier
{
    public int Priority => 1100;

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length != 6 || syllableCounts.Length != 6)
        {
            definition = null;
            return false;
        }

        if (
            syllableCounts[0] == 5
            && syllableCounts[1] == 7
            && syllableCounts[2] == 7
            && syllableCounts[3] == 5
            && syllableCounts[4] == 7
            && syllableCounts[5] == 7
        )
        {
            definition = new PoemDefinition
            {
                Type = PoemType.Sedoka,
                LineCount = 6,
                SyllablesPerLine = [5, 7, 7, 5, 7, 7],
                TotalSyllableCount = 38,
                WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
                TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
            };
            return true;
        }

        definition = null;
        return false;
    }
}
