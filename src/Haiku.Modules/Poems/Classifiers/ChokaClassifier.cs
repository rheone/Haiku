using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the choka (long poem) form: an odd number of lines (minimum 7) with alternating
/// 5-7 syllable counts, ending with a 5-7-7 closing triplet.
/// </summary>
public sealed class ChokaClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 1400;

    /// <summary>
    /// Gets the type metadata for the choka form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the alternating 5-7 long-form Japanese poem.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Choka,
            DisplayName: "Choka",
            Description: "A long poem with alternating 5-7 syllable lines, ending with 5-7-7. Always an odd number of lines.",
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
        var n = lines.Length;

        if (n < 7 || n % 2 != 1)
        {
            definition = null;
            return false;
        }

        // Alternating 5-7 for all lines except the last three which form 5-7-7
        for (var i = 0; i <= n - 4; i++)
        {
            if (syllableCounts[i] != (i % 2 == 0 ? 5 : 7))
            {
                definition = null;
                return false;
            }
        }

        if (syllableCounts[n - 3] != 5 || syllableCounts[n - 2] != 7 || syllableCounts[n - 1] != 7)
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
