namespace Haiku.Modules.Poems.Classifiers;

/// <summary>
/// Priority-ordered chain of classifiers. First match wins.
/// Also acts as the central registry of all known poem types.
/// </summary>
public sealed class PoemClassifierChain
{
    private readonly IPoemClassifier[] _classifiers;
    private readonly Dictionary<string, IPoemClassifier> _byTypeId;
    private readonly Dictionary<PoemType, IPoemClassifier> _byPoemType;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PoemClassifierChain"/> class,
    ///     sorting classifiers by priority and building lookup dictionaries for fast
    ///     access by <see cref="PoemType"/> and TypeId string.
    /// </summary>
    /// <param name="classifiers">The set of classifiers to register.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when two classifiers have the same <see cref="IPoemClassifier.Priority"/> value.
    /// </exception>
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

    /// <summary>Gets all registered classifiers (ordered by priority).</summary>
    /// <value>A read-only view of the classifier array, sorted by ascending <see cref="IPoemClassifier.Priority"/>.</value>
    public IReadOnlyCollection<IPoemClassifier> AllTypes => _classifiers;

    /// <summary>Looks up a classifier by its TypeId string.</summary>
    /// <param name="typeId">The classifier's unique type identifier (kebab-case, e.g. "haiku").</param>
    /// <returns>The matching classifier, or <c>null</c> if not found.</returns>
    public IPoemClassifier? GetType(string typeId) => _byTypeId.GetValueOrDefault(typeId);

    /// <summary>
    /// Looks up a classifier by its <see cref="PoemType"/> enum value.
    /// </summary>
    /// <param name="poemType">The poem type enum value to look up.</param>
    /// <returns>The matching classifier, or <c>null</c> if not found.</returns>
    public IPoemClassifier? GetType(PoemType poemType) => _byPoemType.GetValueOrDefault(poemType);

    /// <summary>
    /// Runs all classifiers in priority order and returns the first matching definition.
    /// Returns a Freeform definition if nothing matches.
    /// </summary>
    /// <param name="lines">The poem lines to classify.</param>
    /// <param name="syllableCounts">The pre-computed syllable count for each line.</param>
    /// <param name="tokenizedLines">The tokenized representation of each line.</param>
    /// <returns>
    /// The matching <see cref="PoemDefinition"/> from the first classifier that succeeds,
    /// or a minimal Freeform definition if none match.
    /// </returns>
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
