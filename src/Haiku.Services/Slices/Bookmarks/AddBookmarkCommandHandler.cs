using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Bookmarks;

/// <summary>
/// Handles adding a bookmark. Idempotent: returns <c>false</c> if the bookmark already exists.
/// </summary>
public class AddBookmarkCommandHandler : ICommandHandler<AddBookmarkCommand, bool>
{
    private readonly IBookmarkRepository _bookmarkRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddBookmarkCommandHandler"/> class.
    /// </summary>
    /// <param name="bookmarkRepository">The bookmark repository for data access.</param>
    public AddBookmarkCommandHandler(IBookmarkRepository bookmarkRepository)
    {
        _bookmarkRepository = bookmarkRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> Handle(AddBookmarkCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existing = await _bookmarkRepository.GetByUserAndPoemAsync(request.UserId, request.PoemId, cancellationToken);
        if (existing != null)
        {
            return false;
        }

        var bookmark = new Bookmark
        {
            Id = Guid.NewGuid(),
            // EF Core stub: sets the FK by attaching a reference without loading the full entity.
            User = new User { Id = request.UserId },
            Poem = new Poem { Id = request.PoemId },
            CreatedAt = DateTime.UtcNow,
        };

        await _bookmarkRepository.SaveAsync(bookmark, cancellationToken);
        return true;
    }
}
