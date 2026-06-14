using Haiku.Domain.Enums;

namespace Haiku.Services.Poems.Classifiers.SequenceHelpers;

internal static class PatternGenerator
{
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

    public static int[] GeneratePiPattern(Random rng)
    {
        var lineCount = rng.Next(3, 21);
        return PiSequence.GetDigits(lineCount);
    }

    public static int[] GenerateFibPattern(Random rng)
    {
        var lineCount = rng.Next(3, 11);
        return FibonacciSequence.GetTerms(lineCount);
    }

    public static int[] GenerateReverseFibPattern(Random rng)
    {
        var lineCount = rng.Next(3, 11);
        return FibonacciSequence.GetReverseTerms(lineCount);
    }

    public static int[] GenerateWavePattern(Random rng)
    {
        var halfSteps = rng.Next(2, 5);
        var length = halfSteps * 2 + 1;
        var n = rng.Next(2, 6);
        return BuildWave(length, n);
    }

    public static int[] GenerateCrestWavePattern(Random rng)
    {
        var numWaves = rng.Next(2, 4);
        var halfSteps = rng.Next(2, 4);
        var waveLen = halfSteps * 2 + 1;
        var firstBase = rng.Next(numWaves + 2, numWaves + 5);
        return BuildMultiWavePattern(numWaves, waveLen, firstBase, ascending: false);
    }

    public static int[] GenerateCrashWavePattern(Random rng)
    {
        var numWaves = rng.Next(2, 4);
        var halfSteps = rng.Next(2, 4);
        var waveLen = halfSteps * 2 + 1;
        var firstBase = rng.Next(2, 5);
        return BuildMultiWavePattern(numWaves, waveLen, firstBase, ascending: true);
    }

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

    public static int[] GenerateCollatzPattern(Random rng)
    {
        var start = rng.Next(3, 21);
        return CollatzSequence.Generate(start);
    }

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
