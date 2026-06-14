using System.Collections.Concurrent;
using System.Reflection;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;

namespace Haiku.Services.Poems.Classifiers.SequenceHelpers;

/// <summary>
/// Builds <see cref="PoemDefinition"/> instances from classifier metadata.
/// Every classifier should use this builder instead of constructing the record directly.
/// </summary>
/// <remarks>
/// <para>Metadata is read from the classifier's <c>static abstract</c> <see cref="PoemTypeInfo"/>
/// via a single-reflection-per-type cache, so the classifier instance does not carry
/// redundant metadata properties.</para>
/// <para>The <see cref="PoemType"/> enum is the single source of truth for type identity.
/// Classifiers pass the enum value to <see cref="PoemTypeInfo"/> and the <see cref="PoemTypeInfo.TypeId"/>
/// is derived automatically via kebab-case conversion. No manual string-to-enum mapping is needed
/// anywhere in the system.</para>
/// </remarks>
internal static class ClassifierBuilder
{
    private static readonly ConcurrentDictionary<Type, PoemTypeInfo> InfoCache = new();

    /// <summary>
    /// Gets the <see cref="PoemTypeInfo"/> for a classifier instance (cached per type).
    /// </summary>
    public static PoemTypeInfo GetInfo(IPoemClassifier classifier) =>
        InfoCache.GetOrAdd(
            classifier.GetType(),
            static type =>
            {
                var prop = type.GetProperty("Info", BindingFlags.Public | BindingFlags.Static);
                if (prop is null)
                {
                    throw new InvalidOperationException(
                        $"Classifier {type.Name} does not implement static abstract Info property."
                    );
                }
                return (PoemTypeInfo)prop.GetValue(null)!;
            }
        );

    /// <summary>
    /// Creates a fully-populated <see cref="PoemDefinition"/> from a classifier instance.
    /// </summary>
    public static PoemDefinition Build(IPoemClassifier classifier)
    {
        var info = GetInfo(classifier);
        return new PoemDefinition
        {
            TypeId = info.TypeId,
            DisplayName = info.DisplayName,
            Description = info.Description,
            Category = info.Category,
            Scaffold = info.Scaffold,
            SyllablePattern = info.SyllablePattern,
            WordPattern = info.WordPattern,
            Type = info.PoemType,
            Name = info.DisplayName,
        };
    }
}
