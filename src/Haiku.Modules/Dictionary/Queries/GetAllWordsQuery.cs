using MicroMediator;

namespace Haiku.Modules.Dictionary.Queries;

/// <summary>
/// Retrieves all custom dictionary words.
/// </summary>
public record GetAllWordsQuery : IQuery<List<CustomDictionaryWord>>;
