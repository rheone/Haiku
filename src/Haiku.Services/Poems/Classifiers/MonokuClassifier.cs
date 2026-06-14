using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class MonokuClassifier : IPoemClassifier
{
    public int Priority => 100;

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        var totalSyllables = syllableCounts.Sum();

        if (lines.Length == 1 && totalSyllables >= 4 && totalSyllables <= 17)
        {
            definition = new PoemDefinition
            {
                Type = PoemType.Monoku,
                LineCount = 1,
                SyllablesPerLine = syllableCounts,
                TotalSyllableCount = totalSyllables,
                WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
                TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
            };
            return true;
        }

        definition = null;
        return false;
    }
}
