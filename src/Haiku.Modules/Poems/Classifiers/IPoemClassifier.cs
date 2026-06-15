using System.Diagnostics.CodeAnalysis;

namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Classifies a poem into a specific type. Each implementation is a self-contained
/// type definition providing both detection logic AND display metadata.
/// </summary>
/// <remarks>
/// <para>To add a new poem type:</para>
/// <list type="number">
///   <item>Add a value to the <see cref="PoemType"/> enum in <c>Haiku.Domain</c>.</item>
///   <item>Implement this interface with a <c>TypeId</c> that matches the enum name
///     converted to kebab-case (e.g. <c>SyllableCrestWave</c> → <c>"syllable-crest-wave"</c>).</item>
///   <item>The classifier is auto-discovered via DI assembly scanning — no manual
///     registration needed.</item>
/// </list>
/// <para><b>TypeId convention:</b> The <see cref="PoemTypeInfo.TypeId"/> is derived
/// automatically from the <see cref="PoemType"/> enum using kebab-case conversion.
/// Pass the enum value to <see cref="PoemTypeInfo"/> and the TypeId follows — no manual
/// string is accepted. The <c>ClassifierBuilder</c> reads <c>Info.PoemType</c>
/// directly, eliminating any mapping step.</para>
/// <para>Static metadata is available via <c>TClassifier.Info</c> without needing
/// an instance (e.g. <c>HaikuClassifier.Info.TypeId</c>).</para>
/// </remarks>
public interface IPoemClassifier
{
    /// <summary>Gets the detection priority (lower runs first). Must be unique across all classifiers.</summary>
    /// <value>The ordinal priority value; classifiers with lower values are evaluated first.</value>
    int Priority { get; }

    /// <summary>
    /// Attempts to classify the given poem lines into this type.
    /// </summary>
    /// <param name="lines">The poem lines to classify.</param>
    /// <param name="syllableCounts">The pre-computed syllable count for each line.</param>
    /// <param name="tokenizedLines">The tokenized representation of each line.</param>
    /// <param name="definition">
    /// When successful, the <see cref="PoemDefinition"/> for this type populated with display metadata;
    /// <c>null</c> when the lines do not match this classifier.
    /// </param>
    /// <returns><c>true</c> if the lines match this poem type; <c>false</c> otherwise.</returns>
    bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    );
}
