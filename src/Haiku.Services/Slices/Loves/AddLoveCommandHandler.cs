using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Loves;

/// <summary>
/// Handles adding a love (like) to a poem, guarding against duplicate loves.
/// </summary>
public class AddLoveCommandHandler : ICommandHandler<AddLoveCommand, bool>
{
    private readonly ILoveRepository _loveRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddLoveCommandHandler"/> class.
    /// </summary>
    /// <param name="loveRepository">The love repository for data access.</param>
    public AddLoveCommandHandler(ILoveRepository loveRepository)
    {
        _loveRepository = loveRepository;
    }

    /// <inheritdoc/>
    /// <returns><c>true</c> if the love was recorded; <c>false</c> if the user already loved this poem.</returns>
    /// <remarks>
    /// <para>
    /// Returns <c>false</c> if the user has already loved the poem (duplicate guard).
    /// Otherwise creates a new <see cref="Love"/> entity in the database.
    /// </para>
    /// </remarks>
    public async Task<bool> Handle(AddLoveCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existing = await _loveRepository.GetByUserAndPoemAsync(request.UserId, request.PoemId, cancellationToken);
        if (existing != null)
        {
            return false;
        }

        var love = new Love
        {
            Id = Guid.NewGuid(),
            // EF Core stub: sets the FK by attaching a reference without loading the full entity.
            Poem = new Poem { Id = request.PoemId },
            User = new User { Id = request.UserId },
            CreatedAt = DateTime.UtcNow,
        };

        await _loveRepository.SaveAsync(love, cancellationToken);
        return true;
    }
}
