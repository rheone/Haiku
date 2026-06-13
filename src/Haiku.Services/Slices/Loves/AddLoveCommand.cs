using MicroMediator;

namespace Haiku.Services.Slices.Loves;

/// <summary>
/// Command to record a "love" (like) from a user on a poem.
/// </summary>
/// <param name="PoemId">The identifier of the poem to love.</param>
/// <param name="UserId">The identifier of the user giving the love.</param>
public record AddLoveCommand(Guid PoemId, Guid UserId) : ICommand<bool>;
