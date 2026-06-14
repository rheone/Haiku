using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class CompressedClassifier : IPoemClassifier
{
    public int Priority => 600;

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

        if (syllableCounts[0] == 2 && syllableCounts[1] == 3 && syllableCounts[2] == 2)
        {
            definition = new PoemDefinition
            {
                Type = PoemType.Compressed,
                LineCount = 3,
                SyllablesPerLine = [2, 3, 2],
                TotalSyllableCount = 7,
                WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
                TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
            };
            return true;
        }

        definition = null;
        return false;
    }
}
