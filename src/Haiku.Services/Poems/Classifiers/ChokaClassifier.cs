using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class ChokaClassifier : IPoemClassifier
{
    public int Priority => 1400;

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        var n = lines.Length;

        if (n < 7 || n % 2 != 1)
        {
            definition = null;
            return false;
        }

        // Alternating 5-7 for all lines except the last three which form 5-7-7
        for (var i = 0; i <= n - 4; i++)
        {
            if (syllableCounts[i] != (i % 2 == 0 ? 5 : 7))
            {
                definition = null;
                return false;
            }
        }

        if (syllableCounts[n - 3] != 5 || syllableCounts[n - 2] != 7 || syllableCounts[n - 1] != 7)
        {
            definition = null;
            return false;
        }

        definition = new PoemDefinition
        {
            Type = PoemType.Choka,
            LineCount = n,
            SyllablesPerLine = syllableCounts,
            TotalSyllableCount = syllableCounts.Sum(),
            WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
            TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
        };
        return true;
    }
}
