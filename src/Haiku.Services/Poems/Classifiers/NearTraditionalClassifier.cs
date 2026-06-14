using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class NearTraditionalClassifier : IPoemClassifier
{
    public int Priority => 700;

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length != 3 || syllableCounts.Length != 3)
        {
            definition = null;
            return false;
        }

        if (syllableCounts[0] == 4 && syllableCounts[1] == 6 && syllableCounts[2] == 4)
        {
            definition = new PoemDefinition
            {
                Type = PoemType.NearTraditional,
                LineCount = 3,
                SyllablesPerLine = [4, 6, 4],
                TotalSyllableCount = 14,
                WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
                TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
            };
            return true;
        }

        definition = null;
        return false;
    }
}
