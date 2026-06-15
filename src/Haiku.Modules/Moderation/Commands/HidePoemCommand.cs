using MicroMediator;

namespace Haiku.Modules.Moderation.Commands;

/// <summary>
/// Command to hide a poem as a moderation action.
/// </summary>
/// <param name="PoemId">The identifier of the poem to hide.</param>
/// <param name="ActionedByUserId">The identifier of the moderator performing the action.</param>
/// <param name="Reason">The reason for hiding the poem.</param>
public record HidePoemCommand(Guid PoemId, Guid ActionedByUserId, string Reason) : ICommand<bool>;
