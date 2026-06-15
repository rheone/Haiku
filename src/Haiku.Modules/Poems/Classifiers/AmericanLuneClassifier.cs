using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the American Lune form: exactly three lines with a 3-5-3 syllable pattern.
/// A modern American haiku adaptation, formerly known as the Minimalist form.
/// </summary>
public sealed class AmericanLuneClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 400;

    /// <summary>
    /// Gets the type metadata for the American Lune form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the 3-5-3 syllable-based modern American form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.AmericanLune,
            DisplayName: "American Lune",
            Description: "A three-line modern American adaptation of haiku with 3-5-3 syllable pattern. Formerly called Minimalist.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [3, 5, 3],
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
        if (lines.Length != 3 || syllableCounts.Length != 3)
        {
            definition = null;
            return false;
        }

        if (syllableCounts[0] == 3 && syllableCounts[1] == 5 && syllableCounts[2] == 3)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
