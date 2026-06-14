using System.Diagnostics.CodeAnalysis;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

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
/// string is accepted. The <see cref="ClassifierBuilder"/> reads <c>Info.PoemType</c>
/// directly, eliminating any mapping step.</para>
/// <para>Static metadata is available via <c>TClassifier.Info</c> without needing
/// an instance (e.g. <c>HaikuClassifier.Info.TypeId</c>).</para>
/// </remarks>
public interface IPoemClassifier
{
    /// <summary>Detection priority (lower runs first). Must be unique across all classifiers.</summary>
    int Priority { get; }

    /// <summary>
    /// Attempts to classify the given poem lines into this type.
    /// </summary>
    bool TryClassify(
        string[] lines,
        int[] syllableCounts,
        TokenizedLine[] tokenizedLines,
        [NotNullWhen(true)] out PoemDefinition? definition
    );
}
