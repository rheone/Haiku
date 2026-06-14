using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Word Pulse form: per-line word counts alternate between
/// two distinct values. Minimum 4 lines, even count.
/// </summary>
public sealed class WordPulseClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3100;

    /// <summary>
    /// Gets the type metadata for the Word Pulse form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the alternating two-value word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordPulse,
            DisplayName: "Word Pulse",
            Description: "Word counts alternate between two distinct values (a, b, a, b, ...). Min 4 lines, even.",
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

        if (!PatternMatchers.IsPulse(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
