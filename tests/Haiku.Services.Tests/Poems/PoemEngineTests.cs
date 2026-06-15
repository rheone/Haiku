using System.Text.Json;
using NewSyllableEngine = Haiku.Modules.Poems.Syllables.SyllableEngine;
using ServicesPoemEngine = Haiku.Modules.Poems.Application.PoemEngine;

namespace Haiku.Services.Tests.Poems;

public class PoemEngineTests
{
    #region Analyze

    /// <summary>
    /// Placeholder: verifies that Analyze returns a result without throwing.
    /// </summary>
    [Fact]
    public void Analyze_ReturnsResultWithoutThrowing_Test()
    {
        // Arrange
        var poemEngine = new ServicesPoemEngine();

        // Act
        var result = poemEngine.Analyze("hello world");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Lines);
    }

    #endregion
}

public class PatternGeneratorTests
{
    #region General

    /// <summary>
    /// Verifies that GeneratePattern returns a non-empty integer pattern for the given PoemType.
    /// </summary>
    [Theory]
    [InlineData(PoemType.SyllablePi)]
    [InlineData(PoemType.SyllableFib)]
    [InlineData(PoemType.SyllableReverseFib)]
    [InlineData(PoemType.SyllableWave)]
    [InlineData(PoemType.SyllableCrestWave)]
    [InlineData(PoemType.SyllableCrashWave)]
    [InlineData(PoemType.SyllablePrime)]
    [InlineData(PoemType.SyllablePulse)]
    [InlineData(PoemType.SyllableHailstone)]
    [InlineData(PoemType.SyllableStair)]
    [InlineData(PoemType.SyllableErosion)]
    [InlineData(PoemType.SyllableMountain)]
    [InlineData(PoemType.SyllableNautilus)]
    [InlineData(PoemType.WordPi)]
    [InlineData(PoemType.WordFib)]
    [InlineData(PoemType.WordReverseFib)]
    [InlineData(PoemType.WordWave)]
    [InlineData(PoemType.WordCrestWave)]
    [InlineData(PoemType.WordCrashWave)]
    [InlineData(PoemType.WordPrime)]
    [InlineData(PoemType.WordPulse)]
    [InlineData(PoemType.WordHailstone)]
    [InlineData(PoemType.WordStair)]
    [InlineData(PoemType.WordErosion)]
    [InlineData(PoemType.WordMountain)]
    [InlineData(PoemType.WordNautilus)]
    public void GeneratePattern_ReturnsNonEmptyPattern_Test(PoemType type)
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GeneratePattern(type, rng);

        // Assert
        Assert.NotNull(pattern);
        Assert.True(pattern.Length >= 2, $"Pattern for {type} should have at least 2 elements, got {pattern.Length}");
        foreach (var val in pattern)
        {
            Assert.True(val >= 1, $"All values in {type} pattern should be >= 1, got {val}");
        }
    }

    #endregion

    #region Specific Patterns

    /// <summary>
    /// Verifies that GeneratePiPattern returns the expected number of Pi digits.
    /// </summary>
    [Fact]
    public void GeneratePattern_Pi_ReturnsPiDigits_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GeneratePiPattern(rng);

        // Assert
        Assert.True(pattern.Length >= 3 && pattern.Length <= 20);
        var expected = PiSequence.GetDigits(pattern.Length);
        Assert.Equal(expected, pattern);
    }

    /// <summary>
    /// Verifies that GenerateFibPattern returns the expected number of Fibonacci terms.
    /// </summary>
    [Fact]
    public void GeneratePattern_Fib_ReturnsFibTerms_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GenerateFibPattern(rng);

        // Assert
        Assert.True(pattern.Length >= 3 && pattern.Length <= 10);
        var expected = FibonacciSequence.GetTerms(pattern.Length);
        Assert.Equal(expected, pattern);
    }

    /// <summary>
    /// Verifies that GenerateReverseFibPattern returns the expected reversed Fibonacci terms.
    /// </summary>
    [Fact]
    public void GeneratePattern_ReverseFib_ReturnsReverseFibTerms_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GenerateReverseFibPattern(rng);

        // Assert
        Assert.True(pattern.Length >= 3 && pattern.Length <= 10);
        var expected = FibonacciSequence.GetReverseTerms(pattern.Length);
        Assert.Equal(expected, pattern);
    }

    /// <summary>
    /// Verifies that GenerateWavePattern produces a symmetric wave pattern with odd length.
    /// </summary>
    [Fact]
    public void GeneratePattern_Wave_IsSymmetric_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GenerateWavePattern(rng);

        // Assert
        Assert.True(pattern.Length >= 5 && pattern.Length % 2 == 1);
        Assert.Equal(pattern[0], pattern[^1]);
        var mid = pattern.Length / 2;
        for (var i = 1; i <= mid; i++)
        {
            Assert.True(pattern[i] >= pattern[i - 1]);
        }
        for (var i = mid + 1; i < pattern.Length; i++)
        {
            Assert.True(pattern[i] <= pattern[i - 1]);
        }
    }

    /// <summary>
    /// Verifies that all values in a prime pattern are prime numbers.
    /// </summary>
    [Fact]
    public void GeneratePattern_Prime_AllValuesArePrime_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GeneratePrimePattern(rng);

        // Assert
        Assert.True(pattern.Length >= 3 && pattern.Length <= 8);
        foreach (var val in pattern)
        {
            Assert.True(PrimeHelper.IsPrime(val), $"{val} should be prime");
        }
    }

    /// <summary>
    /// Verifies that GeneratePulsePattern produces an alternating two-value sequence.
    /// </summary>
    [Fact]
    public void GeneratePattern_Pulse_Alternates_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GeneratePulsePattern(rng);

        // Assert
        Assert.True(pattern.Length >= 4 && pattern.Length % 2 == 0);
        var a = pattern[0];
        var b = pattern[1];
        Assert.NotEqual(a, b);
        for (var i = 0; i < pattern.Length; i++)
        {
            Assert.Equal(i % 2 == 0 ? a : b, pattern[i]);
        }
    }

    /// <summary>
    /// Verifies that GenerateCollatzPattern ends with 1 and matches the Collatz sequence.
    /// </summary>
    [Fact]
    public void GeneratePattern_Collatz_EndsWithOne_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GenerateCollatzPattern(rng);

        // Assert
        Assert.True(pattern.Length >= 3);
        Assert.Equal(1, pattern[^1]);
        var expected = CollatzSequence.Generate(pattern[0]);
        Assert.Equal(expected, pattern);
    }

    /// <summary>
    /// Verifies that GenerateStairPattern produces a strictly incrementing sequence.
    /// </summary>
    [Fact]
    public void GeneratePattern_Stair_IncrementsByOne_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GenerateStairPattern(rng);

        // Assert
        Assert.True(pattern.Length >= 3 && pattern.Length <= 8);
        for (var i = 1; i < pattern.Length; i++)
        {
            Assert.Equal(pattern[i - 1] + 1, pattern[i]);
        }
    }

    /// <summary>
    /// Verifies that GenerateErosionPattern produces a decreasing sequence ending at 1.
    /// </summary>
    [Fact]
    public void GeneratePattern_Erosion_DecrementsToOne_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GenerateErosionPattern(rng);

        // Assert
        Assert.True(pattern.Length >= 3);
        Assert.Equal(1, pattern[^1]);
        for (var i = 1; i < pattern.Length; i++)
        {
            Assert.Equal(pattern[i - 1] - 1, pattern[i]);
        }
    }

    /// <summary>
    /// Verifies that GenerateMountainPattern starts at 1 and increments by one each step.
    /// </summary>
    [Fact]
    public void GeneratePattern_Mountain_StartsAtOne_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GenerateMountainPattern(rng);

        // Assert
        Assert.True(pattern.Length >= 3 && pattern.Length <= 10);
        Assert.Equal(1, pattern[0]);
        for (var i = 1; i < pattern.Length; i++)
        {
            Assert.Equal(pattern[i - 1] + 1, pattern[i]);
        }
    }

    /// <summary>
    /// Verifies that GenerateNautilusPattern has a constant second difference of 1.
    /// </summary>
    [Fact]
    public void GeneratePattern_Nautilus_HasConstantSecondDifference_Test()
    {
        // Arrange
        var rng = new Random(42);

        // Act
        var pattern = PatternGenerator.GenerateNautilusPattern(rng);

        // Assert
        Assert.True(pattern.Length >= 3 && pattern.Length <= 8);
        var d1 = pattern[1] - pattern[0];
        var d2 = pattern[2] - pattern[1];
        Assert.Equal(d1 + 1, d2);
    }

    #endregion

    #region Edge Cases

    /// <summary>
    /// Verifies that the same seed produces identical patterns.
    /// </summary>
    [Fact]
    public void GeneratePattern_SeededDeterminism_Test()
    {
        // Arrange & Act
        var pattern1 = PatternGenerator.GeneratePattern(PoemType.SyllablePi, new Random(42));
        var pattern2 = PatternGenerator.GeneratePattern(PoemType.SyllablePi, new Random(42));

        // Assert
        Assert.Equal(pattern1, pattern2);
    }

    /// <summary>
    /// Verifies that an unknown PoemType returns an empty pattern.
    /// </summary>
    [Fact]
    public void GeneratePattern_UnknownType_ReturnsEmpty_Test()
    {
        // Act
        var pattern = PatternGenerator.GeneratePattern(PoemType.Freeform, new Random(42));

        // Assert
        Assert.Empty(pattern);
    }

    #endregion
}

public sealed class PoemEngineGenerationTests : IDisposable
{
    private readonly string _testJsonPath;
    private readonly ServicesPoemEngine _engine;

    public PoemEngineGenerationTests()
    {
        _testJsonPath = Path.GetTempFileName();
        var json = """
            {
              "_metadata": {
                "source": "https://github.com/cmusphinx/cmudict",
                "commit": "test-fixture",
                "generatedAt": "2026-06-14T00:00:00Z",
                "license": "Public Domain"
              },
              "entries": {
                "hello": [{ "s": 2, "p": ["HH", "AH0", "L", "OW1"] }],
                "world": [{ "s": 1, "p": ["W", "ER1", "L", "D"] }],
                "silence": [{ "s": 2, "p": ["S", "AY1", "L", "AH0", "N", "S"] }],
                "record": [
                  { "s": 2, "p": ["R", "EH1", "K", "ER0", "D"] },
                  { "s": 3, "p": ["R", "IH0", "K", "AO1", "R", "D"] }
                ],
                "the": [{ "s": 1, "p": ["DH", "AH0"] }],
                "and": [{ "s": 1, "p": ["AE1", "N", "D"] }],
                "one": [{ "s": 1, "p": ["W", "AH1", "N"] }],
                "two": [{ "s": 1, "p": ["T", "UW1"] }],
                "three": [{ "s": 1, "p": ["TH", "R", "IY1"] }],
                "four": [{ "s": 1, "p": ["F", "AO1", "R"] }],
                "five": [{ "s": 1, "p": ["F", "AY1", "V"] }],
                "six": [{ "s": 1, "p": ["S", "IH1", "K", "S"] }],
                "seven": [{ "s": 2, "p": ["S", "EH1", "V", "AH0", "N"] }],
                "eight": [{ "s": 1, "p": ["EY1", "T"] }],
                "nine": [{ "s": 1, "p": ["N", "AY1", "N"] }],
                "ten": [{ "s": 1, "p": ["T", "EH1", "N"] }],
                "happy": [{ "s": 2, "p": ["HH", "AE1", "P", "IY0"] }],
                "sunny": [{ "s": 2, "p": ["S", "AH1", "N", "IY0"] }],
                "little": [{ "s": 2, "p": ["L", "IH1", "T", "AH0", "L"] }],
                "gentle": [{ "s": 2, "p": ["JH", "EH1", "N", "T", "AH0", "L"] }],
                "morning": [{ "s": 2, "p": ["M", "AO1", "R", "N", "IH0", "NG"] }],
                "evening": [{ "s": 3, "p": ["IY1", "V", "AH0", "N", "IH0", "NG"] }],
                "beautiful": [{ "s": 3, "p": ["B", "YW1", "T", "AH0", "F", "AH0", "L"] }],
                "wonderful": [{ "s": 3, "p": ["W", "AH1", "N", "D", "ER0", "F", "AH0", "L"] }],
                "butterfly": [{ "s": 3, "p": ["B", "AH1", "T", "ER0", "F", "L", "AY2"] }],
                "rainbow": [{ "s": 2, "p": ["R", "EY1", "N", "B", "OW2"] }],
                "crystal": [{ "s": 2, "p": ["K", "R", "IH1", "S", "T", "AH0", "L"] }],
                "whisper": [{ "s": 2, "p": ["W", "IH1", "S", "P", "ER0"] }],
                "silver": [{ "s": 2, "p": ["S", "IH1", "L", "V", "ER0"] }],
                "golden": [{ "s": 2, "p": ["G", "OW1", "L", "D", "AH0", "N"] }],
                "welcome": [{ "s": 2, "p": ["W", "EH1", "L", "K", "AH0", "M"] }],
                "always": [{ "s": 2, "p": ["AO1", "L", "W", "EY2", "Z"] }]
              }
            }
            """;
        File.WriteAllText(_testJsonPath, json);

        var cmuDictionary = new CmuDictionaryProvider(_testJsonPath);
        var tokenizer = new WordTokenizer();
        var providers = new ISyllableProvider[] { cmuDictionary, new HeuristicSyllableProvider() };
        var syllableEngine = new SyllableEngine(providers, tokenizer);

        var assembly = typeof(ServicesPoemEngine).Assembly;
        var classifierTypes = assembly
            .GetExportedTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IPoemClassifier).IsAssignableFrom(t));
        var classifiers = classifierTypes.Select(t => (IPoemClassifier)Activator.CreateInstance(t)!).ToList();
        var chain = new PoemClassifierChain(classifiers);

        _engine = new ServicesPoemEngine(chain, syllableEngine, cmuDictionary, null);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (File.Exists(_testJsonPath))
            File.Delete(_testJsonPath);
    }

    #region GeneratePoem

    /// <summary>
    /// Verifies that GeneratePoem returns the correct number of lines for a traditional poem type (Haiku).
    /// </summary>
    [Fact]
    public void GeneratePoem_TraditionalType_ReturnsCorrectLineCount_Test()
    {
        // Act
        var poem = _engine.GeneratePoem(PoemType.Haiku, seed: 42);

        // Assert
        Assert.Equal(3, poem.Length);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    /// <summary>
    /// Verifies that GeneratePoem returns a non-empty result for SyllablePi type.
    /// </summary>
    [Fact]
    public void GeneratePoem_SyllablePi_ReturnsNonEmpty_Test()
    {
        // Act
        var poem = _engine.GeneratePoem(PoemType.SyllablePi, seed: 42);

        // Assert
        Assert.True(poem.Length >= 3);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    /// <summary>
    /// Verifies that GeneratePoem returns a non-empty result for WordPi type.
    /// </summary>
    [Fact]
    public void GeneratePoem_WordPi_ReturnsNonEmpty_Test()
    {
        // Act
        var poem = _engine.GeneratePoem(PoemType.WordPi, seed: 42);

        // Assert
        Assert.True(poem.Length >= 3);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    /// <summary>
    /// Verifies that GeneratePoem returns a non-empty result for SyllableFib type.
    /// </summary>
    [Fact]
    public void GeneratePoem_SyllableFib_ReturnsNonEmpty_Test()
    {
        // Act
        var poem = _engine.GeneratePoem(PoemType.SyllableFib, seed: 42);

        // Assert
        Assert.True(poem.Length >= 3);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    /// <summary>
    /// Verifies that GeneratePoem returns a non-empty result for WordFib type.
    /// </summary>
    [Fact]
    public void GeneratePoem_WordFib_ReturnsNonEmpty_Test()
    {
        // Act
        var poem = _engine.GeneratePoem(PoemType.WordFib, seed: 42);

        // Assert
        Assert.True(poem.Length >= 3);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    /// <summary>
    /// Verifies that all non-traditional poem types generate at least 2 non-empty lines.
    /// </summary>
    [Fact]
    public void GeneratePoem_AllNonTraditionalTypes_ReturnNonEmpty_Test()
    {
        // Arrange
        var nonTraditional = new[]
        {
            PoemType.SyllablePi,
            PoemType.WordPi,
            PoemType.SyllableFib,
            PoemType.WordFib,
            PoemType.SyllableReverseFib,
            PoemType.WordReverseFib,
            PoemType.SyllableWave,
            PoemType.WordWave,
            PoemType.SyllableCrestWave,
            PoemType.WordCrestWave,
            PoemType.SyllableCrashWave,
            PoemType.WordCrashWave,
            PoemType.SyllablePrime,
            PoemType.WordPrime,
            PoemType.SyllablePulse,
            PoemType.WordPulse,
            PoemType.SyllableHailstone,
            PoemType.WordHailstone,
            PoemType.SyllableStair,
            PoemType.WordStair,
            PoemType.SyllableErosion,
            PoemType.WordErosion,
            PoemType.SyllableMountain,
            PoemType.WordMountain,
            PoemType.SyllableNautilus,
            PoemType.WordNautilus,
        };

        // Assert
        foreach (var type in nonTraditional)
        {
            // Act
            var poem = _engine.GeneratePoem(type, seed: 42);

            Assert.True(poem.Length >= 2, $"{type} should generate at least 2 lines, got {poem.Length}");
            foreach (var line in poem)
            {
                Assert.False(string.IsNullOrWhiteSpace(line), $"{type} should not contain empty lines");
            }
        }
    }

    /// <summary>
    /// Verifies that GeneratePoem returns an empty result for Freeform type.
    /// </summary>
    [Fact]
    public void GeneratePoem_Freeform_ReturnsEmpty_Test()
    {
        // Act
        var poem = _engine.GeneratePoem(PoemType.Freeform, seed: 42);

        // Assert
        Assert.Empty(poem);
    }

    /// <summary>
    /// Verifies that the same seed produces identical poems.
    /// </summary>
    [Fact]
    public void GeneratePoem_SeededDeterminism_Test()
    {
        // Arrange & Act
        var poem1 = _engine.GeneratePoem(PoemType.SyllablePi, seed: 42);
        var poem2 = _engine.GeneratePoem(PoemType.SyllablePi, seed: 42);

        // Assert
        Assert.Equal(poem1, poem2);
    }

    /// <summary>
    /// Verifies that GeneratePoem returns exactly one line for Monoku type.
    /// </summary>
    [Fact]
    public void GeneratePoem_Monoku_ReturnsOneLine_Test()
    {
        // Act
        var poem = _engine.GeneratePoem(PoemType.Monoku, seed: 42);

        // Assert
        Assert.Single(poem);
        Assert.False(string.IsNullOrWhiteSpace(poem[0]));
    }

    /// <summary>
    /// Verifies that GeneratePoem returns 2-5 non-empty lines for Isosyllabic type.
    /// </summary>
    [Fact]
    public void GeneratePoem_Isosyllabic_AllLinesSameLength_Test()
    {
        // Act
        var poem = _engine.GeneratePoem(PoemType.Isosyllabic, seed: 42);

        // Assert
        Assert.True(poem.Length >= 2 && poem.Length <= 5);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    #endregion
}
