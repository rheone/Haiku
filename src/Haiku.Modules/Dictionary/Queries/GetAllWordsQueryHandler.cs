using MicroMediator;

namespace Haiku.Modules.Dictionary.Queries;

/// <summary>
/// Handles retrieval of all custom dictionary words by delegating to the repository.
/// </summary>
public class GetAllWordsQueryHandler : IQueryHandler<GetAllWordsQuery, List<CustomDictionaryWord>>
{
    private readonly IDictionaryRepository _dictionaryRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllWordsQueryHandler"/> class.
    /// </summary>
    /// <param name="dictionaryRepository">The dictionary repository for data access.</param>
    public GetAllWordsQueryHandler(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    /// <inheritdoc/>
    public async Task<List<CustomDictionaryWord>> Handle(GetAllWordsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _dictionaryRepository.GetAllWordsAsync(cancellationToken);
    }
}
