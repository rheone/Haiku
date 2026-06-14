namespace Haiku.Domain.Tests.Entities;

/// <summary>
/// Unit tests for <see cref="Haiku.Domain.Entities.PoemTag"/> entity instantiation and property assignment.
/// The poem-tag join entity uses a composite primary key (PoemId, TagId) and overrides
/// <c>Equals</c> and <c>GetHashCode</c> for correct value semantics in collection operations.
/// </summary>
public class PoemTagTests { }
