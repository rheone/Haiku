using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the compressed haiku-inspired form: exactly three lines with a 2-3-2 syllable pattern.
/// </summary>
public sealed class CompressedClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 600;

    /// <summary>
    /// Gets the type metadata for the compressed haiku-inspired form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the 2-3-2 syllable-based ultra-short form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Compressed,
            DisplayName: "Compressed",
            Description: "A three-line nonstandard haiku-inspired ultra-short form with 2-3-2 syllable pattern.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [2, 3, 2],
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

        if (syllableCounts[0] == 2 && syllableCounts[1] == 3 && syllableCounts[2] == 2)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
