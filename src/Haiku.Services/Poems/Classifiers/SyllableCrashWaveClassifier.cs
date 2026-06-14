using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Detects the Syllable Crash Wave form: two or more syllable wave patterns chained
/// end-to-end, each successive wave having a larger peak. Minimum 10 lines.
/// </summary>
public sealed class SyllableCrashWaveClassifier : IPoemClassifier
{
    /// <inheritdoc/>
    public int Priority => 2600;

    /// <summary>
    /// Gets the type metadata for the Syllable Crash Wave form.
    /// </summary>
    /// <value>A <see cref="PoemTypeInfo"/> describing the growing-peak wave chain pattern.</value>
    public static PoemTypeInfo Info { get; } =
        new(
            PoemType: PoemType.SyllableCrashWave,
            DisplayName: "Syllable Crash Wave",
            Description: "Two or more syllable waves chained, each with a larger peak. Same shape. Min 10 lines.",
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
        if (!WaveClassifierBase.IsCrashWave(syllableCounts))
        {
            definition = null;
            return false;
        }

        definition = ClassifierBuilder.Build(this);
        return true;
    }
}
