using MicroMediator;

namespace Haiku.Modules.Moderation.Commands;

/// <summary>
/// Command to unhide a previously hidden poem.
/// </summary>
/// <param name="PoemId">The identifier of the poem to unhide.</param>
/// <param name="ActionedByUserId">The identifier of the moderator performing the action.</param>
/// <param name="Reason">The reason for unhiding the poem.</param>
public record UnhidePoemCommand(Guid PoemId, Guid ActionedByUserId, string Reason) : ICommand<bool>;
