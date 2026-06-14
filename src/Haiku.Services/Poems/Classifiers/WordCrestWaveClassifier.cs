using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Word Crest Wave form: two or more word wave patterns chained
/// end-to-end, each successive wave having a smaller peak. Minimum 10 lines.
/// </summary>
public sealed class WordCrestWaveClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 2500;

    /// <summary>
    /// Gets the type metadata for the Word Crest Wave form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the diminishing-peak wave chain pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordCrestWave,
            DisplayName: "Word Crest Wave",
            Description: "Two or more word waves chained, each with a smaller peak. Same shape. Min 10 lines.",
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

        if (!WaveClassifierBase.IsCrestWave(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
