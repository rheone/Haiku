using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class MirrorCinquainClassifier : IPoemClassifier
{
    public int Priority => 1300;

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length != 10 || syllableCounts.Length != 10)
        {
            definition = null;
            return false;
        }

        if (
            syllableCounts[0] == 2
            && syllableCounts[1] == 4
            && syllableCounts[2] == 6
            && syllableCounts[3] == 8
            && syllableCounts[4] == 2
            && syllableCounts[5] == 2
            && syllableCounts[6] == 8
            && syllableCounts[7] == 6
            && syllableCounts[8] == 4
            && syllableCounts[9] == 2
        )
        {
            definition = new PoemDefinition
            {
                Type = PoemType.MirrorCinquain,
                LineCount = 10,
                SyllablesPerLine = [2, 4, 6, 8, 2, 2, 8, 6, 4, 2],
                TotalSyllableCount = 44,
                WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
                TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
            };
            return true;
        }

        definition = null;
        return false;
    }
}
