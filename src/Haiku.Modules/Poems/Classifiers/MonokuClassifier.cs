using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the monoku form: a single-line poem with total syllable count between 4 and 17 inclusive.
/// The highest-priority classifier, evaluated first in the chain.
/// </summary>
public sealed class MonokuClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 100;

    /// <summary>
    /// Gets the type metadata for the monoku form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the single-line syllable-range-based form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Monoku,
            DisplayName: "Monoku",
            Description: "A single-line poem where total syllables must be between 4 and 17 inclusive.",
            Category: PoemCategory.Traditional,
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
        var totalSyllables = syllableCounts.Sum();

        if (lines.Length == 1 && totalSyllables >= 4 && totalSyllables <= 17)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
