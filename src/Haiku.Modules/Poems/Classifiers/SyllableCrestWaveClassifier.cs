using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Crest Wave form: two or more syllable wave patterns chained
/// end-to-end, each successive wave having a smaller peak. Minimum 10 lines.
/// </summary>
public sealed class SyllableCrestWaveClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 2400;

    /// <summary>
    /// Gets the type metadata for the Syllable Crest Wave form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the diminishing-peak wave chain pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableCrestWave,
            DisplayName: "Syllable Crest Wave",
            Description: "Two or more syllable waves chained, each with a smaller peak. Same shape. Min 10 lines.",
            Category: PoemCategory.NonTraditional,
            Scaffold: PoemScaffold.SyllableBased,
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
        if (!WaveClassifierBase.IsCrestWave(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
