using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Bookmarks;

/// <summary>
/// Handles removing a bookmark. Idempotent: returns <c>false</c> if the bookmark does not exist.
/// </summary>
public class RemoveBookmarkCommandHandler : ICommandHandler<RemoveBookmarkCommand, bool>
{
    private readonly IBookmarkRepository _bookmarkRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveBookmarkCommandHandler"/> class.
    /// </summary>
    /// <param name="bookmarkRepository">The bookmark repository for data access.</param>
    public RemoveBookmarkCommandHandler(IBookmarkRepository bookmarkRepository)
    {
        _bookmarkRepository = bookmarkRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(RemoveBookmarkCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existing = await _bookmarkRepository.GetByUserAndPoemAsync(request.UserId, request.PoemId, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        await _bookmarkRepository.DeleteAsync(existing, cancellationToken);
        return true;
    }
}
