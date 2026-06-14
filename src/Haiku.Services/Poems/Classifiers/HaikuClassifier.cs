using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class HaikuClassifier : IPoemClassifier
{
    public int Priority => 200;

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        out PoemDefinition? definition
    )
    {
        if (lines.Length != 3 || syllableCounts.Length != 3)
        {
            definition = null;
            return false;
        }

        if (syllableCounts[0] == 5 && syllableCounts[1] == 7 && syllableCounts[2] == 5)
        {
            definition = new PoemDefinition
            {
                Type = PoemType.Haiku,
                LineCount = 3,
                SyllablesPerLine = [5, 7, 5],
                TotalSyllableCount = 17,
                WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
                TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
            };
            return true;
        }

        definition = null;
        return false;
    }
}
