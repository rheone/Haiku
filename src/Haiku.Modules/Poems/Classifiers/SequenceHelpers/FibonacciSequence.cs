namespace Haiku.Modules.Poems.Classifiers.SequenceHelpers;

/// <summary>
/// Provides Fibonacci sequence terms for poem type detection.
/// Terms grow exponentially; the 500-char content limit naturally
/// bounds usable terms to roughly the first 12-15.
/// </summary>
/// <remarks>
/// <para>Hardcoded limit: 20 terms. Term 20 (6765) is far beyond what
/// can fit in <c>nvarchar(500)</c>, but the full array is provided for
/// completeness. If the maximum poem size is increased, the boundary
/// comment should be revisited.</para>
/// </remarks>
internal static class FibonacciSequence
{
    private static readonly int[] Terms =
    [
        1,
        1,
        2,
        3,
        5,
        8,
        13,
        21,
        34,
        55,
        89,
        144,
        233,
        377,
        610,
        987,
        1597,
        2584,
        4181,
        6765,
    ];

    /// <summary>
    /// Returns the first <paramref name="count"/> Fibonacci terms (F1=1, F2=1, ...).
    /// </summary>
    /// <param name="count">The number of Fibonacci terms to return.</param>
    /// <returns>An array of the first <paramref name="count"/> Fibonacci terms.</returns>
    public static int[] GetTerms(int count)
    {
        if (count <= 0)
        {
            return [];
        }

        if (count >= Terms.Length)
        {
            return Terms.ToArray();
        }

        return Terms[..count];
    }

    /// <summary>
    /// Returns the first <paramref name="count"/> Fibonacci terms in reverse order
    /// (last term first, descending to 1).
    /// </summary>
    /// <param name="count">The number of Fibonacci terms to reverse and return.</param>
    /// <returns>An array of the first <paramref name="count"/> Fibonacci terms in descending order.</returns>
    public static int[] GetReverseTerms(int count)
    {
        var forward = GetTerms(count);
        Array.Reverse(forward);
        return forward;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> matches a prefix of
    /// the Fibonacci sequence (1, 1, 2, 3, 5, ...). Min 3 elements.
    /// </summary>
    /// <param name="counts">The per-line syllable or word counts to compare.</param>
    /// <returns><c>true</c> if the counts match a prefix of the Fibonacci sequence; otherwise <c>false</c>.</returns>
    public static bool IsFibMatch(int[] counts)
    {
        if (counts.Length < 3)
        {
            return false;
        }

        var expected = GetTerms(counts.Length);
        if (expected.Length < counts.Length)
        {
            return false;
        }

        for (var i = 0; i < counts.Length; i++)
        {
            if (counts[i] != expected[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> matches a prefix of
    /// the reversed Fibonacci sequence (largest term first, descending to 1, 1).
    /// Min 3 elements.
    /// </summary>
    /// <param name="counts">The per-line syllable or word counts to compare.</param>
    /// <returns><c>true</c> if the counts match a prefix of the reversed Fibonacci sequence; otherwise <c>false</c>.</returns>
    public static bool IsReverseFibMatch(int[] counts)
    {
        if (counts.Length < 3)
        {
            return false;
        }

        var expected = GetReverseTerms(counts.Length);
        if (expected.Length < counts.Length)
        {
            return false;
        }

        for (var i = 0; i < counts.Length; i++)
        {
            if (counts[i] != expected[i])
            {
                return false;
            }
        }

        return true;
    }
}
