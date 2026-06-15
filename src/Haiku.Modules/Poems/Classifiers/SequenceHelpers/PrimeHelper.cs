namespace Haiku.Modules.Poems.Classifiers.SequenceHelpers;

/// <summary>
/// Simple primality check for small integers used in poem classification.
/// </summary>
internal static class PrimeHelper
{
    private static readonly HashSet<int> SmallPrimes =
    [
        2,
        3,
        5,
        7,
        11,
        13,
        17,
        19,
        23,
        29,
        31,
        37,
        41,
        43,
        47,
        53,
        59,
        61,
        67,
        71,
        73,
        79,
        83,
        89,
        97,
    ];

    /// <summary>
    /// Returns <c>true</c> if <paramref name="n"/> is a prime number.
    /// </summary>
    /// <param name="n">The integer to test for primality.</param>
    /// <returns><c>true</c> if <paramref name="n"/> is prime; otherwise <c>false</c>.</returns>
    public static bool IsPrime(int n)
    {
        if (n < 2)
        {
            return false;
        }

        if (SmallPrimes.Contains(n))
        {
            return true;
        }

        if (n <= 100)
        {
            return false;
        }

        var limit = (int)Math.Sqrt(n);
        for (var i = 2; i <= limit; i++)
        {
            if (n % i == 0)
            {
                return false;
            }
        }

        return true;
    }
}
