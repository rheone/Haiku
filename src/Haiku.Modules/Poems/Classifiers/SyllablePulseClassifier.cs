using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Pulse form: per-line syllable counts alternate between
/// two distinct values. Minimum 4 lines, even count.
/// </summary>
public sealed class SyllablePulseClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3000;

    /// <summary>
    /// Gets the type metadata for the Syllable Pulse form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the alternating two-value syllable pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllablePulse,
            DisplayName: "Syllable Pulse",
            Description: "Syllable counts alternate between two distinct values (a, b, a, b, ...). Min 4 lines, even.",
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
        if (!PatternMatchers.IsPulse(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
