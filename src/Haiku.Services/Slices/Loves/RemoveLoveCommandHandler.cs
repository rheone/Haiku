using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Loves;

/// <summary>
/// Handles removing a love (like) from a poem.
/// </summary>
public class RemoveLoveCommandHandler : ICommandHandler<RemoveLoveCommand, bool>
{
    private readonly ILoveRepository _loveRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveLoveCommandHandler"/> class.
    /// </summary>
    /// <param name="loveRepository">The love repository for data access.</param>
    public RemoveLoveCommandHandler(ILoveRepository loveRepository)
    {
        _loveRepository = loveRepository;
    }

    /// <inheritdoc/>
    /// <returns><c>true</c> if the love was removed; <c>false</c> if no love existed for this user and poem.</returns>
    /// <remarks>
    /// <para>
    /// Returns <c>false</c> if no love exists for the given user and poem combination.
    /// Otherwise deletes the <c>Love</c> entity from the database.
    /// </para>
    /// </remarks>
    public async Task<bool> Handle(RemoveLoveCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existing = await _loveRepository.GetByUserAndPoemAsync(request.UserId, request.PoemId, cancellationToken);
        if (existing == null)
        {
            return false;
        }

        await _loveRepository.DeleteAsync(existing, cancellationToken);
        return true;
    }
}
