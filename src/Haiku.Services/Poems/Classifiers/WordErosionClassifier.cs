using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Word Erosion form: per-line word counts decrease by exactly 1
/// each line, descending to 1. Minimum 3 lines.
/// </summary>
public sealed class WordErosionClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3700;

    /// <summary>
    /// Gets the type metadata for the Word Erosion form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the descending-count word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordErosion,
            DisplayName: "Word Erosion",
            Description: "Word counts decrease by exactly 1 each line: n, n-1, ..., 1. Min 3 lines, last line = 1.",
            Category: PoemCategory.NonTraditional,
            Scaffold: PoemScaffold.WordBased,
            SyllablePattern: null,
            WordPattern: null
        );

    /// <inheritdoc/>
    public bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    )
    {
        var wordCounts = tokenizedLines.Select(t => t.WordCount).ToArray();

        if (!PatternMatchers.IsErosion(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
