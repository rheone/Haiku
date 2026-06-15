using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Stair form: per-line syllable counts increase by exactly 1
/// each line. Minimum 3 lines.
/// </summary>
public sealed class SyllableStairClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3400;

    /// <summary>
    /// Gets the type metadata for the Syllable Stair form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the stair-ascending syllable pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableStair,
            DisplayName: "Syllable Stair",
            Description: "Syllable counts increase by exactly 1 each line: n, n+1, n+2, ... Min 3 lines.",
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
        if (!PatternMatchers.IsStair(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
