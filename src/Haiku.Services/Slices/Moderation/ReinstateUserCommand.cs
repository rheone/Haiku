using MicroMediator;

namespace Haiku.Services.Slices.Moderation;

/// <summary>
/// Command to reinstate a previously disabled user account.
/// </summary>
/// <param name="UserId">The identifier of the user to reinstate.</param>
/// <param name="ActionedByUserId">The identifier of the moderator performing the action.</param>
/// <param name="Reason">The reason for the reinstatement.</param>
public record ReinstateUserCommand(Guid UserId, Guid ActionedByUserId, string Reason) : ICommand<bool>;
