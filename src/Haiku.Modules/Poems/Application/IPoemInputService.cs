namespace Haiku.Modules.Poems.Application;

/// <summary>Provides processing of raw poem input, including normalization, syllable counting, and validation.</summary>
public interface IPoemInputService
{
    /// <summary>Processes raw poem content and returns a normalized result with syllable counts and errors.</summary>
    /// <param name="rawContent">The unprocessed poem text.</param>
    /// <returns>A <see cref="PoemInputResult"/> containing the normalized content, line data, and any validation errors.</returns>
    PoemInputResult Process(string rawContent);
}
