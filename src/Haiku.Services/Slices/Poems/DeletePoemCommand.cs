using MicroMediator;

namespace Haiku.Services.Slices.Poems;

/// <summary>
/// Command to soft-delete a poem by its identifier.
/// </summary>
/// <param name="PoemId">The identifier of the poem to delete.</param>
public record DeletePoemCommand(Guid PoemId) : ICommand<bool>;
