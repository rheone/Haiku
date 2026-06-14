using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Poems;

/// <summary>
/// Handles soft-deletion of poems by setting the <c>Poem.DeletedAt</c> timestamp rather than removing rows.
/// </summary>
public class DeletePoemCommandHandler : ICommandHandler<DeletePoemCommand, bool>
{
    private readonly IPoemRepository _poemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletePoemCommandHandler"/> class.
    /// </summary>
    /// <param name="poemRepository">Repository for loading and saving <c>Poem</c> entities.</param>
    public DeletePoemCommandHandler(IPoemRepository poemRepository)
    {
        _poemRepository = poemRepository;
    }

    /// <summary>
    /// Soft-deletes the specified poem. Returns <c>false</c> when no poem exists for the given identifier.
    /// </summary>
    /// <param name="request">The command containing the poem identifier to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the poem was found and soft-deleted; <c>false</c> if the poem does not exist.</returns>
    public async Task<bool> Handle(DeletePoemCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var poem = await _poemRepository.GetByIdAsync(request.PoemId, cancellationToken);
        if (poem == null)
        {
            return false;
        }

        poem.DeletedAt = DateTime.UtcNow;
        await _poemRepository.SaveAsync(poem, cancellationToken);
        return true;
    }
}
