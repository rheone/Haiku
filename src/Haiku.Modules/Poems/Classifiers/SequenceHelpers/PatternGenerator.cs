namespace Haiku.Modules.Poems.Classifiers.SequenceHelpers;

/// <summary>
/// Generates random pattern sequences for poem classification by dispatching
/// on <see cref="PoemType"/> to the appropriate sequence generator.
/// </summary>
internal static class PatternGenerator
{
    /// <summary>
    /// Dispatches to the appropriate pattern generator for the specified <see cref="PoemType"/>.
    /// Returns an empty array for unrecognized types.
    /// </summary>
    /// <param name="type">The poem type whose pattern should be generated.</param>
    /// <param name="rng">The random number generator to use for parameter selection.</param>
    /// <returns>An array of per-line counts representing the pattern, or an empty array if the type is unknown.</returns>
    public static int[] GeneratePattern(PoemType type, Random rng)
    {
        return type switch
        {
            PoemType.SyllablePi or PoemType.WordPi => GeneratePiPattern(rng),
            PoemType.SyllableFib or PoemType.WordFib => GenerateFibPattern(rng),
            PoemType.SyllableReverseFib or PoemType.WordReverseFib => GenerateReverseFibPattern(rng),
            PoemType.SyllableWave or PoemType.WordWave => GenerateWavePattern(rng),
            PoemType.SyllableCrestWave or PoemType.WordCrestWave => GenerateCrestWavePattern(rng),
            PoemType.SyllableCrashWave or PoemType.WordCrashWave => GenerateCrashWavePattern(rng),
            PoemType.SyllablePrime or PoemType.WordPrime => GeneratePrimePattern(rng),
            PoemType.SyllablePulse or PoemType.WordPulse => GeneratePulsePattern(rng),
            PoemType.SyllableHailstone or PoemType.WordHailstone => GenerateCollatzPattern(rng),
            PoemType.SyllableStair or PoemType.WordStair => GenerateStairPattern(rng),
            PoemType.SyllableErosion or PoemType.WordErosion => GenerateErosionPattern(rng),
            PoemType.SyllableMountain or PoemType.WordMountain => GenerateMountainPattern(rng),
            PoemType.SyllableNautilus or PoemType.WordNautilus => GenerateNautilusPattern(rng),
            _ => [],
        };
    }

    /// <summary>
    /// Generates a pattern from consecutive non-zero decimal digits of pi.
    /// Line count is randomly chosen between 3 and 20.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>An array of pi digits for the chosen line count.</returns>
    public static int[] GeneratePiPattern(Random rng)
    {
        var lineCount = rng.Next(3, 21);
        return PiSequence.GetDigits(lineCount);
    }

    /// <summary>
    /// Generates a pattern from the Fibonacci sequence (1, 1, 2, 3, 5, ...).
    /// Line count is randomly chosen between 3 and 10.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>An array of Fibonacci terms for the chosen line count.</returns>
    public static int[] GenerateFibPattern(Random rng)
    {
        var lineCount = rng.Next(3, 11);
        return FibonacciSequence.GetTerms(lineCount);
    }

    /// <summary>
    /// Generates a pattern from the Fibonacci sequence in reverse order
    /// (largest term first, descending to 1, 1). Line count is randomly chosen
    /// between 3 and 10.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>An array of Fibonacci terms in descending order.</returns>
    public static int[] GenerateReverseFibPattern(Random rng)
    {
        var lineCount = rng.Next(3, 11);
        return FibonacciSequence.GetReverseTerms(lineCount);
    }

    /// <summary>
    /// Generates a single symmetric wave pattern rising from a base value to a
    /// peak and descending back. Half-steps (2-4) and base value (2-5) are random.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>An array forming a symmetric wave pattern.</returns>
    public static int[] GenerateWavePattern(Random rng)
    {
        var halfSteps = rng.Next(2, 5);
        var length = (halfSteps * 2) + 1;
        var n = rng.Next(2, 6);
        return BuildWave(length, n);
    }

    /// <summary>
    /// Generates a cresting wave pattern: multiple complete waves where each
    /// subsequent wave has a strictly smaller peak.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>An array forming a descending multi-wave pattern.</returns>
    public static int[] GenerateCrestWavePattern(Random rng)
    {
        var numWaves = rng.Next(2, 4);
        var halfSteps = rng.Next(2, 4);
        var waveLen = (halfSteps * 2) + 1;
        var firstBase = rng.Next(numWaves + 2, numWaves + 5);
        return BuildMultiWavePattern(numWaves, waveLen, firstBase, ascending: false);
    }

    /// <summary>
    /// Generates a crash wave pattern: multiple complete waves where each
    /// subsequent wave has a strictly larger peak.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>An array forming an ascending multi-wave pattern.</returns>
    public static int[] GenerateCrashWavePattern(Random rng)
    {
        var numWaves = rng.Next(2, 4);
        var halfSteps = rng.Next(2, 4);
        var waveLen = (halfSteps * 2) + 1;
        var firstBase = rng.Next(2, 5);
        return BuildMultiWavePattern(numWaves, waveLen, firstBase, ascending: true);
    }

    /// <summary>
    /// Generates a pattern where each element is a randomly selected prime number
    /// from the first eight primes (2, 3, 5, 7, 11, 13, 17, 19). Line count is
    /// randomly chosen between 3 and 8.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>An array of randomly selected prime numbers.</returns>
    public static int[] GeneratePrimePattern(Random rng)
    {
        var lineCount = rng.Next(3, 9);
        var smallPrimes = new[] { 2, 3, 5, 7, 11, 13, 17, 19 };
        var result = new int[lineCount];
        for (var i = 0; i < lineCount; i++)
        {
            result[i] = smallPrimes[rng.Next(smallPrimes.Length)];
        }
        return result;
    }

    /// <summary>
    /// Generates a pulse pattern alternating between two distinct values (a, b, a, b, ...).
    /// Values are randomly chosen from 2-7 and differ from each other.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>An array alternating between two distinct values.</returns>
    public static int[] GeneratePulsePattern(Random rng)
    {
        var halfCycles = rng.Next(2, 6);
        var lineCount = halfCycles * 2;
        var a = rng.Next(2, 8);
        var b = rng.Next(2, 8);
        while (b == a)
        {
            b = rng.Next(2, 8);
        }
        var result = new int[lineCount];
        for (var i = 0; i < lineCount; i++)
        {
            result[i] = i % 2 == 0 ? a : b;
        }
        return result;
    }

    /// <summary>
    /// Generates a Collatz hailstone sequence from a random start value (3-20).
    /// The sequence descends to 1 by applying the Collatz function.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>The Collatz sequence from start down to 1.</returns>
    public static int[] GenerateCollatzPattern(Random rng)
    {
        var start = rng.Next(3, 21);
        return CollatzSequence.Generate(start);
    }

    /// <summary>
    /// Generates a stair pattern where each line increments by 1 from a random
    /// start value (2-5) for a random count (3-8).
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>A linearly increasing sequence.</returns>
    public static int[] GenerateStairPattern(Random rng)
    {
        var start = rng.Next(2, 6);
        var count = rng.Next(3, 9);
        var result = new int[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = start + i;
        }
        return result;
    }

    /// <summary>
    /// Generates an erosion pattern descending from a random start value (5-12)
    /// down to 1 by decrementing by 1 each step.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>A linearly decreasing sequence ending at 1.</returns>
    public static int[] GenerateErosionPattern(Random rng)
    {
        var start = rng.Next(5, 13);
        var result = new int[start];
        for (var i = 0; i < start; i++)
        {
            result[i] = start - i;
        }
        return result;
    }

    /// <summary>
    /// Generates a mountain pattern starting at 1 and incrementing by 1 for a
    /// random count (3-10).
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>A strictly increasing sequence starting from 1.</returns>
    public static int[] GenerateMountainPattern(Random rng)
    {
        var count = rng.Next(3, 11);
        var result = new int[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = i + 1;
        }
        return result;
    }

    /// <summary>
    /// Generates a nautilus pattern with quadratic growth (constant second difference).
    /// Starting value (1-4) and difference delta (1-3) are random. Line count is
    /// randomly chosen between 3 and 8.
    /// </summary>
    /// <param name="rng">The random number generator.</param>
    /// <returns>A quadratically growing sequence.</returns>
    public static int[] GenerateNautilusPattern(Random rng)
    {
        var count = rng.Next(3, 9);
        var a = rng.Next(1, 5);
        var d = rng.Next(1, 4);
        var result = new int[count];
        result[0] = a;
        for (var i = 1; i < count; i++)
        {
            result[i] = result[i - 1] + d + (i - 1);
        }
        return result;
    }

    private static int[] BuildWave(int length, int n)
    {
        var result = new int[length];
        for (var i = 0; i < length; i++)
        {
            result[i] = n + Math.Min(i, length - 1 - i);
        }
        return result;
    }

    private static int[] BuildMultiWavePattern(int numWaves, int waveLen, int firstBase, bool ascending)
    {
        var totalLength = numWaves * waveLen;
        var result = new int[totalLength];
        for (var w = 0; w < numWaves; w++)
        {
            var n = ascending ? firstBase + w : firstBase - w;
            if (n < 1)
            {
                n = 1;
            }
            var offset = w * waveLen;
            for (var i = 0; i < waveLen; i++)
            {
                result[offset + i] = n + Math.Min(i, waveLen - 1 - i);
            }
        }
        return result;
    }
}
