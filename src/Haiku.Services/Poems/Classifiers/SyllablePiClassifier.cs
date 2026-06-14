using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Pi form: each line's syllable count follows the decimal
/// digits of pi (3, 1, 4, 1, 5, 9, ...), skipping zero digits. Minimum 3 lines.
/// </summary>
public sealed class SyllablePiClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 1600;

    /// <summary>
    /// Gets the type metadata for the Syllable Pi form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the pi-digit-matching syllable pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllablePi,
            DisplayName: "Syllable Pi",
            Description: "Each line's syllable count follows the decimal digits of pi (3.14159...), skipping zeros. Min 3 lines.",
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
        if (!PiSequence.IsPiMatch(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
