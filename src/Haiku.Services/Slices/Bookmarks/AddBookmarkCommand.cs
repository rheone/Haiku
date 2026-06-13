using MicroMediator;

namespace Haiku.Services.Slices.Bookmarks;

/// <summary>
/// Bookmarks a poem for a user. Returns <c>false</c> if the bookmark already exists.
/// </summary>
/// <param name="PoemId">The identifier of the poem to bookmark.</param>
/// <param name="UserId">The identifier of the user creating the bookmark.</param>
public record AddBookmarkCommand(Guid PoemId, Guid UserId) : ICommand<bool>;
