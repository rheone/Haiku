using MicroMediator;

namespace Haiku.Services.Slices.Loves;

/// <summary>
/// Command to remove a "love" (like) from a poem.
/// </summary>
/// <param name="PoemId">The identifier of the poem.</param>
/// <param name="UserId">The identifier of the user who gave the love.</param>
public record RemoveLoveCommand(Guid PoemId, Guid UserId) : ICommand<bool>;
