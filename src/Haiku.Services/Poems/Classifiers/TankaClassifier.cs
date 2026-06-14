using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class TankaClassifier : IPoemClassifier
{
    public int Priority => 800;

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
            syllableCounts[0] == 5
            && syllableCounts[1] == 7
            && syllableCounts[2] == 5
            && syllableCounts[3] == 7
            && syllableCounts[4] == 7
        )
        {
            definition = new PoemDefinition
            {
                Type = PoemType.Tanka,
                LineCount = 5,
                SyllablesPerLine = [5, 7, 5, 7, 7],
                TotalSyllableCount = 31,
                WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
                TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
            };
            return true;
        }

        definition = null;
        return false;
    }
}
