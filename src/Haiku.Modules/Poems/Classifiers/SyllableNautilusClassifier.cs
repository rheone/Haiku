using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Nautilus form: per-line syllable counts follow quadratic
/// growth with a constant second difference. Minimum 3 lines.
/// </summary>
public sealed class SyllableNautilusClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 4000;

    /// <summary>
    /// Gets the type metadata for the Syllable Nautilus form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the quadratic-growth syllable pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableNautilus,
            DisplayName: "Syllable Nautilus",
            Description: "Syllable counts follow quadratic growth (constant second difference). Example: 2, 3, 5, 8, 12, ... Min 3 lines.",
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
        if (!PatternMatchers.IsNautilus(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
