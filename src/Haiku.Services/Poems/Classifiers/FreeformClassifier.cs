using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class FreeformClassifier : IPoemClassifier
{
    public int Priority => int.MaxValue;

    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        out PoemDefinition? definition
    )
    {
        definition = new PoemDefinition
        {
            Type = PoemType.Freeform,
            LineCount = lines.Length,
            SyllablesPerLine = syllableCounts,
            TotalSyllableCount = syllableCounts.Sum(),
            WordCountPerLine = tokenizedLines.Select(t => t.WordCount).ToArray(),
            TotalWordCount = tokenizedLines.Sum(t => t.WordCount),
            OriginalContent = string.Join("\n", lines),
            NormalizedContent = string.Join(" ", lines),
        };

        return true;
    }
}
