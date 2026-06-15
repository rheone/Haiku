using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Prime form: every line's syllable count is a prime number.
/// Minimum 3 lines.
/// </summary>
public sealed class SyllablePrimeClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 2800;

    /// <summary>
    /// Gets the type metadata for the Syllable Prime form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the prime-number syllable pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllablePrime,
            DisplayName: "Syllable Prime",
            Description: "Each line's syllable count is a prime number. Min 3 lines.",
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
        if (!PatternMatchers.IsAllPrime(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
