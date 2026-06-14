using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the katauta form: exactly three lines with a 5-7-7 syllable pattern.
/// A classical Japanese form that serves as a half-stanza for the sedoka.
/// </summary>
public sealed class KatautaClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 300;

    /// <summary>
    /// Gets the type metadata for the katauta form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the 5-7-7 syllable-based classical Japanese form.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.Katauta,
            DisplayName: "Katauta",
            Description: "A three-line classical Japanese form with 5-7-7 syllable pattern.",
            Category: PoemCategory.Traditional,
            Scaffold: PoemScaffold.SyllableBased,
            SyllablePattern: [5, 7, 7],
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

        if (syllableCounts[0] == 5 && syllableCounts[1] == 7 && syllableCounts[2] == 7)
        {
            definition = ClassifierBuilder.Build(this);
            return true;
        }

        definition = null;
        return false;
    }
}
