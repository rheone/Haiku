using MicroMediator;

namespace Haiku.Modules.Poems.Commands;

/// <summary>
/// Command to retrieve an existing tag by name or create it if it does not exist.
/// </summary>
/// <param name="TagName">The case-normalized tag name.</param>
public record GetOrCreateTagCommand(string TagName) : ICommand<Tag>;
