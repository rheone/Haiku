using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Haiku.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Haiku.Services;

public class ThemeRecommendationService
{
    private readonly IThemeRepository _themeRepository;
    private readonly ILogger<ThemeRecommendationService> _logger;

    public ThemeRecommendationService(IThemeRepository themeRepository, ILogger<ThemeRecommendationService> logger)
    {
        _themeRepository = themeRepository;
        _logger = logger;
    }

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
