using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Word Mountain form: per-line word counts start at 1 and
/// increase by exactly 1 each line. Minimum 3 lines.
/// </summary>
public sealed class WordMountainClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3900;

    /// <summary>
    /// Gets the type metadata for the Word Mountain form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the sequential-counting word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordMountain,
            DisplayName: "Word Mountain",
            Description: "Word counts increase by exactly 1 starting from 1: 1, 2, 3, ..., n. Min 3 lines.",
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

        if (!PatternMatchers.IsMountain(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
