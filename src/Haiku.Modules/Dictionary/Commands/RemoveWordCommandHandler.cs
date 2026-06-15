using MicroMediator;

namespace Haiku.Modules.Dictionary.Commands;

/// <summary>
/// Handles removal of a word from the custom dictionary. Idempotent: returns <c>false</c> if the word is not found.
/// </summary>
public class RemoveWordCommandHandler : ICommandHandler<RemoveWordCommand, bool>
{
    private readonly IDictionaryRepository _dictionaryRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveWordCommandHandler"/> class.
    /// </summary>
    /// <param name="dictionaryRepository">The dictionary repository for data access.</param>
    public RemoveWordCommandHandler(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(RemoveWordCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existing = await _dictionaryRepository.GetWordAsync(request.Word, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        await _dictionaryRepository.DeleteWordAsync(existing, cancellationToken);
        return true;
    }
}
