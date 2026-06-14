using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Syllables;

namespace Haiku.Services.Poems.Classifiers;

public sealed class PoemClassifierChain
{
    private readonly IPoemClassifier[] _classifiers;

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
    }

    public PoemDefinition Match(string[] lines, int[] syllableCounts, TokenizedLine[] tokenizedLines)
    {
        foreach (var classifier in _classifiers)
        {
            if (classifier.TryClassify(lines, syllableCounts, tokenizedLines, out var definition))
            {
                return definition;
            }
        }

        return new PoemDefinition { Type = PoemType.Freeform };
    }
}
