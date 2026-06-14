using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

/// <summary>
/// Priority-ordered chain of classifiers. First match wins.
/// Also acts as the central registry of all known poem types.
/// </summary>
public sealed class PoemClassifierChain
{
    private readonly IPoemClassifier[] _classifiers;
    private readonly Dictionary<string, IPoemClassifier> _byTypeId;
    private readonly Dictionary<PoemType, IPoemClassifier> _byPoemType;

    public PoemClassifierChain(IEnumerable<IPoemClassifier> classifiers)
    {
        _classifiers = classifiers.OrderBy(c => c.Priority).ToArray();

        for (var i = 1; i < _classifiers.Length; i++)
        {
            if (_classifiers[i].Priority == _classifiers[i - 1].Priority)
            {
                throw new InvalidOperationException(
                    $"Duplicate classifier priority {_classifiers[i].Priority} detected for "
                        + $"{_classifiers[i].GetType().Name} and {_classifiers[i - 1].GetType().Name}."
                );
            }
        }

        // Build TypeId and PoemType lookups; skip classifiers that don't implement
        // static Info (e.g. test mocks). In production all registered classifiers have it.
        _byTypeId = new Dictionary<string, IPoemClassifier>(StringComparer.OrdinalIgnoreCase);
        _byPoemType = new Dictionary<PoemType, IPoemClassifier>();
        foreach (var c in _classifiers)
        {
            var info = TryGetInfo(c);
            if (info is not null)
            {
                _byTypeId[info.TypeId] = c;
                _byPoemType[info.PoemType] = c;
            }
        }
    }

    /// <summary>All registered classifiers (ordered by priority).</summary>
    public IReadOnlyCollection<IPoemClassifier> AllTypes => _classifiers;

    /// <summary>Looks up a classifier by its TypeId string.</summary>
    public IPoemClassifier? GetType(string typeId) => _byTypeId.GetValueOrDefault(typeId);

    /// <summary>
    /// Looks up a classifier by its <see cref="PoemType"/> enum.
    /// </summary>
    public IPoemClassifier? GetType(PoemType poemType) => _byPoemType.GetValueOrDefault(poemType);

    /// <summary>
    /// Runs all classifiers in priority order and returns the first matching definition.
    /// Returns a Freeform definition if nothing matches.
    /// </summary>
    public PoemDefinition Match(string[] lines, int[] syllableCounts, TokenizedLine[] tokenizedLines)
    {
        foreach (var classifier in _classifiers)
        {
            if (classifier.TryClassify(lines, syllableCounts, tokenizedLines, out var definition))
            {
                return definition;
            }
        }

        // Fallback: use the FreeformClassifier (last in priority order)
        var fallback = _classifiers[^1];
        try
        {
            return ClassifierBuilder.Build(fallback);
        }
        catch
        {
            // If the last classifier doesn't implement static Info (e.g. test mocks),
            // return a minimal Freeform definition.
            return new PoemDefinition
            {
                TypeId = "freeform",
                DisplayName = "Freeform",
                Category = PoemCategory.Traditional,
                Scaffold = PoemScaffold.SyllableBased,
                Type = PoemType.Freeform,
                Name = "Freeform",
            };
        }
    }

    private static PoemTypeInfo? TryGetInfo(IPoemClassifier classifier)
    {
        try
        {
            return ClassifierBuilder.GetInfo(classifier);
        }
        catch
        {
            return null;
        }
    }
}
