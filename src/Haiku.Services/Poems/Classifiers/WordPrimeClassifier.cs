using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Word Prime form: every line's word count is a prime number.
/// Minimum 3 lines.
/// </summary>
public sealed class WordPrimeClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 2900;

    /// <summary>
    /// Gets the type metadata for the Word Prime form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the prime-number word pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordPrime,
            DisplayName: "Word Prime",
            Description: "Each line's word count is a prime number. Min 3 lines.",
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

        if (!PatternMatchers.IsAllPrime(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
