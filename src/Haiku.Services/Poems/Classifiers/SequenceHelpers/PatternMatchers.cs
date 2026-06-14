namespace Haiku.Services.Poems.Classifiers.SequenceHelpers;

/// <summary>
/// Static detection methods for non-sequence poem pattern families.
/// Each method takes an <c>int[]</c> of per-line counts (syllables or words)
/// and returns <c>true</c> if the pattern matches.
/// </summary>
/// <remarks>
/// <para>To add a new pattern-based poem type, add a static method here and
/// create a thin classifier that delegates to it. The convention keeps
/// pattern logic testable in one place and classifiers as simple adapters.</para>
/// </remarks>
internal static class PatternMatchers
{
    /// <summary>
    /// Returns <c>true</c> if every element in <paramref name="counts"/> is a prime number.
    /// Min 3 elements. Uses <see cref="PrimeHelper.IsPrime"/>.
    /// </summary>
    public static bool IsAllPrime(int[] counts)
    {
        if (counts.Length < 3)
        {
            return false;
        }

        foreach (var c in counts)
        {
            if (!PrimeHelper.IsPrime(c))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> alternates between two
    /// distinct values (a, b, a, b, ...). Requires an even number of elements
    /// with at least 4. Both a and b must be >= 1, and a != b.
    /// </summary>
    public static bool IsPulse(int[] counts)
    {
        var len = counts.Length;
        if (len < 4 || len % 2 != 0)
        {
            return false;
        }

        var a = counts[0];
        var b = counts[1];

        if (a < 1 || b < 1 || a == b)
        {
            return false;
        }

        for (var i = 0; i < len; i++)
        {
            var expected = i % 2 == 0 ? a : b;
            if (counts[i] != expected)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> follows the Collatz sequence
    /// from <c>counts[0]</c> down to 1. Min 3 elements, last must be 1.
    /// Uses <see cref="CollatzSequence.Generate"/>.
    /// </summary>
    public static bool IsCollatzMatch(int[] counts)
    {
        if (counts.Length < 3)
        {
            return false;
        }

        var start = counts[0];
        if (start < 1)
        {
            return false;
        }

        var expected = CollatzSequence.Generate(start);
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

        return counts[^1] == 1;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> increases by exactly 1
    /// each line: n, n+1, n+2, ..., n+k. Min 3 elements, n >= 1.
    /// </summary>
    public static bool IsStair(int[] counts)
    {
        if (counts.Length < 3)
        {
            return false;
        }

        var n = counts[0];
        if (n < 1)
        {
            return false;
        }

        for (var i = 0; i < counts.Length; i++)
        {
            if (counts[i] != n + i)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> decreases by exactly 1
    /// each line: n, n-1, n-2, ..., 1. Min 3 elements, n >= 3, last must be 1.
    /// </summary>
    public static bool IsErosion(int[] counts)
    {
        if (counts.Length < 3)
        {
            return false;
        }

        var n = counts[0];
        if (n < 3)
        {
            return false;
        }

        for (var i = 0; i < counts.Length; i++)
        {
            if (counts[i] != n - i)
            {
                return false;
            }
        }

        return counts[^1] == 1;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> increases by exactly 1
    /// starting from 1: 1, 2, 3, ..., n. Min 3 elements, first must be 1.
    /// </summary>
    public static bool IsMountain(int[] counts)
    {
        if (counts.Length < 3)
        {
            return false;
        }

        if (counts[0] != 1)
        {
            return false;
        }

        for (var i = 0; i < counts.Length; i++)
        {
            if (counts[i] != i + 1)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> follows quadratic growth
    /// (constant second difference). Given first two values a, b, the difference
    /// d = b - a must be >= 1. For i >= 2: counts[i] == counts[i-1] + d + (i - 1).
    /// Example: 2, 3, 5, 8, 12, 17, ... Min 3 elements, first >= 1.
    /// </summary>
    public static bool IsNautilus(int[] counts)
    {
        if (counts.Length < 3)
        {
            return false;
        }

        var a = counts[0];
        var b = counts[1];

        if (a < 1 || b <= a)
        {
            return false;
        }

        var d = b - a;

        for (var i = 2; i < counts.Length; i++)
        {
            var expected = counts[i - 1] + d + (i - 1);
            if (counts[i] != expected)
            {
                return false;
            }
        }

        return true;
    }
}
