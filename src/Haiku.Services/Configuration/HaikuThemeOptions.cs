namespace Haiku.Services.Configuration;

/// <summary>
/// Configuration options for the theme recommendation engine.
/// </summary>
public class HaikuThemeOptions
{
    /// <summary>
    /// Gets the minimum confidence score required for a theme recommendation to be returned.
    /// </summary>
    /// <value>The threshold value between 0 and 1. Defaults to 0.55.</value>
    public double RecommendationThreshold { get; init; } = 0.55;
}
