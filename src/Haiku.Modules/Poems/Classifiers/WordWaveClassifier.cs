using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Word Wave form: per-line word counts form a symmetric
/// ascending-then-descending wave pattern with a single peak. Minimum 5 lines, odd.
/// </summary>
public sealed class WordWaveClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 2300;

    /// <summary>
    /// Gets the type metadata for the Word Wave form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the word-wave pattern-based form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.WordWave,
            DisplayName: "Word Wave",
            Description: "Word counts form a symmetric wave: n, n+1, ..., peak, ..., n+1, n. Min 5 lines, odd.",
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

        if (!WaveClassifierBase.IsSingleWave(wordCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
