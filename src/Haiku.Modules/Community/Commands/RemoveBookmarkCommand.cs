using MicroMediator;

namespace Haiku.Modules.Community.Commands;

/// <summary>
/// Removes a bookmark from a poem for a user. Returns <c>false</c> if the bookmark does not exist.
/// </summary>
/// <param name="PoemId">The identifier of the poem to unbookmark.</param>
/// <param name="UserId">The identifier of the user removing the bookmark.</param>
public record RemoveBookmarkCommand(Guid PoemId, Guid UserId) : ICommand<bool>;
