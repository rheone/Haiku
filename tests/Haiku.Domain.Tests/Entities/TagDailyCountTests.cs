namespace Haiku.Domain.Tests.Entities;

/// <summary>
/// Unit tests for <see cref="Haiku.Domain.Entities.TagDailyCount"/> entity instantiation and property assignment.
/// This read-optimized denormalized counter tracks tag usage per calendar date, used for
/// tag cloud displays and trending analysis. The composite primary key is (TagId, Date)
/// with overridden <c>Equals</c> and <c>GetHashCode</c>.
/// </summary>
public class TagDailyCountTests { }
