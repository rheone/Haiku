using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Word Stair form: per-line word counts increase by exactly 1
/// each line. Minimum 3 lines.
/// </summary>
public sealed class WordStairClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3500;

    /// <summary>
    /// Gets the type metadata for the Word Stair form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the stair-ascending word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordStair,
            DisplayName: "Word Stair",
            Description: "Word counts increase by exactly 1 each line: n, n+1, n+2, ... Min 3 lines.",
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

        if (!PatternMatchers.IsStair(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
