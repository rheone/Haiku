using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Word Hailstone form: per-line word counts follow the Collatz
/// (hailstone) sequence from a starting value down to 1. Minimum 3 lines.
/// </summary>
public sealed class WordHailstoneClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3300;

    /// <summary>
    /// Gets the type metadata for the Word Hailstone form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the Collatz-sequence word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordHailstone,
            DisplayName: "Word Hailstone",
            Description: "Word counts follow the Collatz sequence from a starting value down to 1. Min 3 lines, last line = 1.",
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

        if (!PatternMatchers.IsCollatzMatch(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
