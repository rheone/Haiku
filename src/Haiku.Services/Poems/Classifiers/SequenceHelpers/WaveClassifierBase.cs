namespace Haiku.Services.Poems.Classifiers.SequenceHelpers;

/// <summary>
/// Shared wave-detection logic for the Wave classifier family.
/// A single wave is a symmetric sequence: n, n+1, ..., peak, ..., n+1, n.
/// Multi-wave variants (crest, crash) chain waves with same shape but
/// descending or ascending peaks.
/// </summary>
internal static class WaveClassifierBase
{
    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> forms a single symmetric wave.
    /// Min 5 elements, odd length, peak >= n+2, each element at distance d from
    /// the nearer edge equals n + d.
    /// </summary>
    /// <param name="counts">The per-line syllable or word counts to check.</param>
    /// <returns><c>true</c> if the counts form a single symmetric wave; otherwise <c>false</c>.</returns>
    public static bool IsSingleWave(int[] counts)
    {
        var len = counts.Length;
        if (len < 5 || len % 2 != 1)
        {
            return false;
        }

        var n = counts[0];
        if (n < 1)
        {
            return false;
        }

        var peak = n + ((len - 1) / 2);
        if (peak < n + 2)
        {
            return false;
        }

        for (var i = 0; i < len; i++)
        {
            var expected = n + Math.Min(i, len - 1 - i);
            if (counts[i] != expected)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> forms a cresting wave:
    /// two or more complete waves of the same shape, each subsequent wave having
    /// a strictly smaller peak than the previous.
    /// </summary>
    /// <param name="counts">The per-line syllable or word counts to check.</param>
    /// <returns><c>true</c> if the counts form a cresting wave; otherwise <c>false</c>.</returns>
    public static bool IsCrestWave(int[] counts)
    {
        return IsMultiWave(counts, requireDescendingPeaks: true);
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> forms a crashing wave:
    /// two or more complete waves of the same shape, each subsequent wave having
    /// a strictly larger peak than the previous.
    /// </summary>
    /// <param name="counts">The per-line syllable or word counts to check.</param>
    /// <returns><c>true</c> if the counts form a crashing wave; otherwise <c>false</c>.</returns>
    public static bool IsCrashWave(int[] counts)
    {
        return IsMultiWave(counts, requireDescendingPeaks: false);
    }

    /// <summary>
    /// Returns the sub-wave length (number of elements in one wave) given
    /// that the first line is at <paramref name="startIndex"/>.
    /// Scans forward until the value returns to the starting value.
    /// </summary>
    /// <param name="counts">The per-line syllable or word counts to scan.</param>
    /// <param name="startIndex">The index at which the current wave begins.</param>
    /// <returns>The number of elements in the wave starting at <paramref name="startIndex"/>.</returns>
    public static int GetWaveLength(int[] counts, int startIndex)
    {
        var startValue = counts[startIndex];
        var len = counts.Length;

        for (var i = startIndex + 1; i < len; i++)
        {
            if (counts[i] == startValue)
            {
                return i - startIndex + 1;
            }
        }

        return len - startIndex;
    }

    private static bool IsMultiWave(int[] counts, bool requireDescendingPeaks)
    {
        var len = counts.Length;
        if (len < 10)
        {
            return false;
        }

        var waveLength = GetWaveLength(counts, 0);
        if (waveLength < 5 || waveLength % 2 != 1)
        {
            return false;
        }

        if (len % waveLength != 0)
        {
            return false;
        }

        var numWaves = len / waveLength;
        if (numWaves < 2)
        {
            return false;
        }

        var halfSteps = (waveLength - 1) / 2;
        int? previousPeak = null;

        for (var w = 0; w < numWaves; w++)
        {
            var offset = w * waveLength;
            var n = counts[offset];

            if (n < 1)
            {
                return false;
            }

            for (var i = 0; i < waveLength; i++)
            {
                var expected = n + Math.Min(i, waveLength - 1 - i);
                if (counts[offset + i] != expected)
                {
                    return false;
                }
            }

            var peak = n + halfSteps;
            if (peak < n + 2)
            {
                return false;
            }

            if (previousPeak.HasValue)
            {
                if (requireDescendingPeaks && peak >= previousPeak.Value)
                {
                    return false;
                }

                if (!requireDescendingPeaks && peak <= previousPeak.Value)
                {
                    return false;
                }
            }

            previousPeak = peak;
        }

        return true;
    }
}
