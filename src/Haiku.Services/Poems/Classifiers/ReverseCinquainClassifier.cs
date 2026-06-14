using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class ReverseCinquainClassifier : IPoemClassifier
{
    public int Priority => 1000;

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length != 5 || syllableCounts.Length != 5)
        {
            definition = null;
            return false;
        }

        if (
            syllableCounts[0] == 2
            && syllableCounts[1] == 8
            && syllableCounts[2] == 6
            && syllableCounts[3] == 4
            && syllableCounts[4] == 2
        )
        {
            definition = new PoemDefinition
            {
                Type = PoemType.ReverseCinquain,
                LineCount = 5,
                SyllablesPerLine = [2, 8, 6, 4, 2],
                TotalSyllableCount = 22,
                WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
                TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
            };
            return true;
        }

        definition = null;
        return false;
    }
}
