using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Haiku.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Haiku.Services;

/// <summary>
/// Scores poem text against active themes using weighted keyword matching and returns the best match above a confidence threshold.
/// </summary>
public class ThemeRecommendationService
{
    private readonly IThemeRepository _themeRepository;
    private readonly ILogger<ThemeRecommendationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeRecommendationService"/> class.
    /// </summary>
    /// <param name="themeRepository">Repository for theme entities.</param>
    /// <param name="logger">Logger for diagnostic output.</param>
    public ThemeRecommendationService(IThemeRepository themeRepository, ILogger<ThemeRecommendationService> logger)
    {
        _themeRepository = themeRepository;
        _logger = logger;
    }

    /// <summary>
    /// Scores the provided poem text against all active themes and returns the best match.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The algorithm tokenizes the poem text by splitting on whitespace and punctuation,
    /// lowercasing all tokens, and then scoring each active theme by counting keyword matches
    /// weighted by each keyword's <c>Weight</c> property. The raw score is normalized by
    /// total word count to produce a confidence value between 0 and 1.
    /// </para>
    /// <para>
    /// Poems with more than 150 tokens or zero tokens after cleanup receive a confidence of 0.
    /// The default theme is excluded from scoring. If no theme exceeds the <paramref name="threshold"/>,
    /// a zero-confidence recommendation is returned.
    /// </para>
    /// </remarks>
    /// <param name="poemText">The full text of the poem to analyze.</param>
    /// <param name="threshold">Minimum confidence required to return a theme recommendation. Defaults to 0.55.</param>
    /// <returns>A <see cref="ThemeRecommendation"/> containing the best matching theme and confidence score, or a zero-confidence result if no theme meets the threshold.</returns>
    public async Task<ThemeRecommendation> RecommendAsync(string poemText, double threshold = 0.55)
    {
        if (string.IsNullOrWhiteSpace(poemText))
        {
            return new ThemeRecommendation { Confidence = 0 };
        }

        var words = poemText
            .ToLowerInvariant()
            .Split(
                new[]
                {
                    ' ',
                    '\n',
                    '\r',
                    '\t',
                    '.',
                    ',',
                    '!',
                    '?',
                    ';',
                    ':',
                    '"',
                    '\'',
                    '(',
                    ')',
                    '[',
                    ']',
                    '{',
                    '}',
                    '-',
                    '—',
                    '…',
                },
                StringSplitOptions.RemoveEmptyEntries
            )
            .Where(w => w.Length > 0)
            .ToList();

        if (words.Count == 0 || words.Count > 150)
        {
            return new ThemeRecommendation { Confidence = 0 };
        }

        var activeThemes = await _themeRepository.GetActiveAsync();
        var wordCount = words.Count;
        var bestScore = 0.0;
        Theme? bestTheme = null;

        foreach (var theme in activeThemes)
        {
            if (theme.Key == "default")
            {
                continue;
            }

            if (theme.Keywords == null || theme.Keywords.Count == 0)
            {
                continue;
            }

            var score = 0.0;
            foreach (var keyword in theme.Keywords)
            {
                var kw = keyword.Keyword.ToLowerInvariant();
                var matches = words.Count(w => w == kw);
                if (matches > 0)
                {
                    score += matches * keyword.Weight;
                }
            }

            var normalized = wordCount > 0 ? score / wordCount : 0;

            if (normalized > bestScore)
            {
                bestScore = normalized;
                bestTheme = theme;
            }
        }

        if (bestTheme == null || bestScore < threshold)
        {
            return new ThemeRecommendation { Confidence = 0 };
        }

        return new ThemeRecommendation
        {
            ThemeId = bestTheme.ThemeId,
            Confidence = Math.Min(bestScore, 1.0),
            ThemeDisplayName = bestTheme.DisplayName,
        };
    }
}
