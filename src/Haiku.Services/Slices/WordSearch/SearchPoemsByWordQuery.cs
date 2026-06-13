using Haiku.Domain.Entities;
using MicroMediator;

namespace Haiku.Services.Slices.WordSearch;

/// <summary>
/// Query to search for poems whose content contains the specified word.
/// </summary>
/// <param name="Word">The word to search for within poem content.</param>
public record SearchPoemsByWordQuery(string Word) : IQuery<List<Poem>>;
