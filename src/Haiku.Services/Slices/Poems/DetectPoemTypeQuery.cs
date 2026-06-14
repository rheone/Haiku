using Haiku.Domain.Enums;
using MicroMediator;

namespace Haiku.Services.Slices.Poems;

/// <summary>
/// Query to detect the structural poem type (haiku, tanka, monoku, etc.) from content.
/// </summary>
/// <param name="Content">The full text content to analyze.</param>
/// <param name="LineSyllableCounts">Optional pre-computed syllable counts per line. When provided, detection uses these directly instead of invoking <see cref="PoemEngine"/>.</param>
public record DetectPoemTypeQuery(string Content, List<int>? LineSyllableCounts = null) : IQuery<PoemType>;
