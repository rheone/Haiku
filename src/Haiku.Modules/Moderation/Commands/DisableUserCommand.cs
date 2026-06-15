using MicroMediator;

namespace Haiku.Modules.Moderation.Commands;

/// <summary>
/// Command to disable a user account as a moderation action.
/// </summary>
/// <param name="UserId">The identifier of the user to disable.</param>
/// <param name="ActionedByUserId">The identifier of the moderator performing the action.</param>
/// <param name="Reason">The reason for the disable action.</param>
public record DisableUserCommand(Guid UserId, Guid ActionedByUserId, string Reason) : ICommand<bool>;
