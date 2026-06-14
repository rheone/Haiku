namespace Haiku.Services.Poems.Classifiers.SequenceHelpers;

/// <summary>
/// Generates the Collatz (hailstone) sequence for poem type detection.
/// Starting from n, each term is n/2 (if even) or 3n+1 (if odd).
/// The sequence always terminates at the 4-2-1 loop.
/// </summary>
internal static class CollatzSequence
{
    /// <summary>
    /// Generates the Collatz sequence from <paramref name="start"/> down to 1.
    /// </summary>
    public static int[] Generate(int start)
    {
        if (start < 1)
        {
            return [];
        }

        var result = new List<int> { start };

        while (result[^1] != 1)
        {
            var current = result[^1];
            result.Add(current % 2 == 0 ? current / 2 : 3 * current + 1);
        }

        return result.ToArray();
    }
}
