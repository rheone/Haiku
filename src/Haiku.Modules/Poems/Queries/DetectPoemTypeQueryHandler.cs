using MicroMediator;

namespace Haiku.Modules.Poems.Queries;

/// <summary>
/// Handles poem type detection by delegating to <see cref="PoemEngine"/> or applying structural rules on
/// pre-computed syllable counts when <see cref="DetectPoemTypeQuery.LineSyllableCounts"/> is provided.
/// </summary>
public class DetectPoemTypeQueryHandler : IQueryHandler<DetectPoemTypeQuery, PoemType>
{
    private readonly PoemEngine _poemEngine;

    /// <summary>
    /// Initializes a new instance of the <see cref="DetectPoemTypeQueryHandler"/> class.
    /// </summary>
    /// <param name="poemEngine">Engine used for content-based poem type detection.</param>
    public DetectPoemTypeQueryHandler(PoemEngine poemEngine)
    {
        _poemEngine = poemEngine;
    }

    /// <summary>
    /// Detects the poem type from the query content.
    /// </summary>
    /// <param name="request">The query containing content and optional pre-computed syllable counts.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The detected <see cref="PoemType"/>.</returns>
    public Task<PoemType> Handle(DetectPoemTypeQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        PoemType result;

        // Use pre-computed syllable counts when available (avoids redundant engine invocation),
        // otherwise delegate to PoemEngine for content-based parsing and detection.
        if (request.LineSyllableCounts != null)
        {
            result = StaticDetect(request.Content, request.LineSyllableCounts);
        }
        else
        {
            var lines = request.Content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            result = _poemEngine.DetectPoemType(lines) ?? PoemType.Freeform;
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Detects the poem type from content using pre-computed per-line syllable counts without requiring a <see cref="PoemEngine"/> instance.
    /// </summary>
    /// <param name="content">The full poem text.</param>
    /// <param name="lineSyllableCounts">Pre-computed syllable counts for each non-empty line.</param>
    /// <returns>The detected <see cref="PoemType"/>.</returns>
    internal static PoemType StaticDetect(string content, List<int> lineSyllableCounts)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var lineCount = lines.Length;
        var counts = lineSyllableCounts;

        // 1-line poems with 4-17 syllables are classified as Monoku.
        if (lineCount == 1 && counts.Count == 1 && counts[0] >= 4 && counts[0] <= 17)
        {
            return PoemType.Monoku;
        }

        // 3-line poems: check the 6 standard syllable patterns.
        if (lineCount == 3 && counts.Count == 3)
        {
            // 5-7-5 pattern: traditional Haiku.
            if (counts[0] == 5 && counts[1] == 7 && counts[2] == 5)
            {
                return PoemType.Haiku;
            }
            // 5-7-7 pattern: Katauta (open-ended third line).
            if (counts[0] == 5 && counts[1] == 7 && counts[2] == 7)
            {
                return PoemType.Katauta;
            }
            // 3-5-3 pattern: American Lune.
            if (counts[0] == 3 && counts[1] == 5 && counts[2] == 3)
            {
                return PoemType.AmericanLune;
            }
            // 5-3-5 pattern: Kelly Lune.
            if (counts[0] == 5 && counts[1] == 3 && counts[2] == 5)
            {
                return PoemType.KellyLune;
            }
            // 2-3-2 pattern: compressed / minimalist verse.
            if (counts[0] == 2 && counts[1] == 3 && counts[2] == 2)
            {
                return PoemType.Compressed;
            }
            // 4-6-4 pattern: near-traditional (relaxed 5-7-5).
            if (counts[0] == 4 && counts[1] == 6 && counts[2] == 4)
            {
                return PoemType.NearTraditional;
            }
        }

        // 5-line poems: check Tanka and Cinquain variants.
        if (lineCount == 5 && counts.Count == 5)
        {
            // 5-7-5-7-7 pattern: Tanka.
            if (counts[0] == 5 && counts[1] == 7 && counts[2] == 5 && counts[3] == 7 && counts[4] == 7)
            {
                return PoemType.Tanka;
            }
            // 2-4-6-8-2 pattern: American Cinquain.
            if (counts[0] == 2 && counts[1] == 4 && counts[2] == 6 && counts[3] == 8 && counts[4] == 2)
            {
                return PoemType.AmericanCinquain;
            }
            // 2-8-6-4-2 pattern: Reverse Cinquain (syllable counts mirrored).
            if (counts[0] == 2 && counts[1] == 8 && counts[2] == 6 && counts[3] == 4 && counts[4] == 2)
            {
                return PoemType.ReverseCinquain;
            }
        }

        // 6-line poems: 5-7-7-5-7-7 pattern is Sedoka (dialog verse).
        if (lineCount == 6 && counts.Count == 6)
        {
            if (counts[0] == 5 && counts[1] == 7 && counts[2] == 7 && counts[3] == 5 && counts[4] == 7 && counts[5] == 7)
            {
                return PoemType.Sedoka;
            }
        }

        // 9-line poems: 2-4-6-8-2-8-6-4-2 pattern is Butterfly Cinquain.
        if (lineCount == 9 && counts.Count == 9)
        {
            if (
                counts[0] == 2
                && counts[1] == 4
                && counts[2] == 6
                && counts[3] == 8
                && counts[4] == 2
                && counts[5] == 8
                && counts[6] == 6
                && counts[7] == 4
                && counts[8] == 2
            )
            {
                return PoemType.ButterflyCinquain;
            }
        }

        // 10-line poems: 2-4-6-8-2-2-8-6-4-2 pattern is Mirror Cinquain.
        if (lineCount == 10 && counts.Count == 10)
        {
            if (
                counts[0] == 2
                && counts[1] == 4
                && counts[2] == 6
                && counts[3] == 8
                && counts[4] == 2
                && counts[5] == 2
                && counts[6] == 8
                && counts[7] == 6
                && counts[8] == 4
                && counts[9] == 2
            )
            {
                return PoemType.MirrorCinquain;
            }
        }

        // Odd-length poems with 7+ lines: check alternating 5-7 pattern
        // ending with a 5-7-7 closing triplet (Choka structure).
        if (lineCount >= 7 && lineCount % 2 == 1)
        {
            var choka = true;
            for (var i = 0; i < lineCount - 3; i++)
            {
                var expected = i % 2 == 0 ? 5 : 7;
                if (counts[i] != expected)
                {
                    choka = false;
                    break;
                }
            }

            if (choka && counts[^3] == 5 && counts[^2] == 7 && counts[^1] == 7)
            {
                return PoemType.Choka;
            }
        }

        // Poems with 2+ lines where every line has the same syllable count.
        if (lineCount >= 2 && counts.Count == lineCount)
        {
            var first = counts[0];
            if (counts.Skip(1).All(c => c == first))
            {
                return PoemType.Isosyllabic;
            }
        }

        return PoemType.Freeform;
    }
}
