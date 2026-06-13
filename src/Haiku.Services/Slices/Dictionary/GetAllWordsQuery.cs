using Haiku.Domain.Entities;
using MicroMediator;

namespace Haiku.Services.Slices.Dictionary;

/// <summary>
/// Retrieves all custom dictionary words.
/// </summary>
public record GetAllWordsQuery : IQuery<List<CustomDictionaryWord>>;
