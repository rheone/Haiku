using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Dictionary;

/// <summary>
/// Handles direct addition of a word to the custom dictionary (bypasses moderation).
/// Idempotent: returns <c>false</c> if the word already exists.
/// </summary>
public class AddWordCommandHandler : ICommandHandler<AddWordCommand, bool>
{
    private readonly IDictionaryRepository _dictionaryRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddWordCommandHandler"/> class.
    /// </summary>
    /// <param name="dictionaryRepository">The dictionary repository for data access.</param>
    public AddWordCommandHandler(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(AddWordCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existing = await _dictionaryRepository.GetWordAsync(request.Word, cancellationToken);
        if (existing != null)
        {
            return false;
        }

        var entry = new CustomDictionaryWord
        {
            Id = Guid.NewGuid(),
            Word = request.Word,
            SyllableCount = request.SyllableCount,
            // EF Core stub: sets the FK by attaching a reference without loading the full entity.
            AddedBy = new User { Id = request.AddedByUserId },
            AddedAt = DateTime.UtcNow,
            // Bypasses moderation since this is a direct add (not a suggestion).
            IsApproved = true,
        };
        await _dictionaryRepository.SaveWordAsync(entry, cancellationToken);
        return true;
    }
}
