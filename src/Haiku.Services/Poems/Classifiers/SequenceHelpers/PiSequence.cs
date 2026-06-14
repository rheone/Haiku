namespace Haiku.Services.Poems.Classifiers.SequenceHelpers;

/// <summary>
/// Provides the decimal digits of pi (after the decimal point) for poem type
/// detection. Digits equal to zero are skipped per the design spec.
/// </summary>
/// <remarks>
/// <para>Hardcoded limit: 100 decimal digits from pi, yielding ~90 usable
/// values after skipping zeros. This is sufficient for any poem fitting in
/// the current <c>nvarchar(500)</c> content limit. If the maximum poem size
/// is increased, this array must be extended.</para>
/// </remarks>
internal static class PiSequence
{
    // First 100 decimal digits of pi: 3.14159...
    private static readonly int[] RawDigits =
    [
        1,
        4,
        1,
        5,
        9,
        2,
        6,
        5,
        3,
        5,
        8,
        9,
        7,
        9,
        3,
        2,
        3,
        8,
        4,
        6,
        2,
        6,
        4,
        3,
        3,
        8,
        3,
        2,
        7,
        9,
        5,
        0,
        2,
        8,
        8,
        4,
        1,
        9,
        7,
        1,
        6,
        9,
        3,
        9,
        9,
        3,
        7,
        5,
        1,
        0,
        5,
        8,
        2,
        0,
        9,
        7,
        4,
        9,
        4,
        4,
        5,
        9,
        2,
        3,
        0,
        7,
        8,
        1,
        6,
        4,
        0,
        6,
        2,
        8,
        6,
        2,
        0,
        8,
        9,
        9,
        8,
        6,
        2,
        8,
        0,
        3,
        4,
        8,
        2,
        5,
        3,
        4,
        2,
        1,
        1,
        7,
        0,
        6,
        7,
        9,
    ];

    /// <summary>
    /// Returns the first <paramref name="count"/> non-zero decimal digits of pi.
    /// </summary>
    public static int[] GetDigits(int count)
    {
        var result = new List<int>(count);
        foreach (var d in RawDigits)
        {
            if (d == 0)
            {
                continue;
            }

            result.Add(d);
            if (result.Count >= count)
            {
                break;
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="counts"/> matches a prefix of
    /// the pi decimal sequence (first digit after decimal, skipping zeros).
    /// Min 3 elements.
    /// </summary>
    public static bool IsPiMatch(int[] counts)
    {
        if (counts.Length < 3)
        {
            return false;
        }

        var expected = GetDigits(counts.Length);
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
