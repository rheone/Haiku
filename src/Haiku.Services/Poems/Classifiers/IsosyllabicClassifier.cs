using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class IsosyllabicClassifier : IPoemClassifier
{
    public int Priority => 1500;

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        if (lines.Length < 2)
        {
            definition = null;
            return false;
        }

        var first = syllableCounts[0];
        if (first <= 0)
        {
            definition = null;
            return false;
        }

        for (var i = 1; i < syllableCounts.Length; i++)
        {
            if (syllableCounts[i] != first)
            {
                definition = null;
                return false;
            }
        }

        definition = new PoemDefinition
        {
            Type = PoemType.Isosyllabic,
            LineCount = lines.Length,
            SyllablesPerLine = syllableCounts,
            TotalSyllableCount = syllableCounts.Sum(),
            WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
            TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
        };
        return true;
    }
}
