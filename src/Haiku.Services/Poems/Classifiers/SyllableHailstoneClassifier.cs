using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Hailstone form: per-line syllable counts follow the
/// Collatz (hailstone) sequence from a starting value down to 1. Minimum 3 lines.
/// </summary>
public sealed class SyllableHailstoneClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 3200;

    /// <summary>
    /// Gets the type metadata for the Syllable Hailstone form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the Collatz-sequence syllable pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableHailstone,
            DisplayName: "Syllable Hailstone",
            Description: "Syllable counts follow the Collatz sequence from a starting value down to 1. Min 3 lines, last line = 1.",
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
        if (!PatternMatchers.IsCollatzMatch(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
