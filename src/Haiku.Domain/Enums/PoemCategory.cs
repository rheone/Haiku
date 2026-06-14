namespace Haiku.Domain.Enums;

/// <summary>
/// Categories (dimensions) a poem type can belong to.
/// A poem type maps to one value per dimension.
/// </summary>
public enum PoemCategory
{
    /// <summary>Culturally established form (haiku, tanka, cinquain, etc.).</summary>
    Traditional,

    /// <summary>Modern algorithmic or experimental form (pi, fib, wave, etc.).</summary>
    NonTraditional,
}
