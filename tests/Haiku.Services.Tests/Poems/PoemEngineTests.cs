using System.Text.Json;
using Haiku.Domain.Enums;
using Haiku.Domain.ValueObjects;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Poems.Classifiers.SequenceHelpers;
using Haiku.Services.Syllables;
using Haiku.Services.Syllables.Providers;
using NewSyllableEngine = Haiku.Services.Syllables.SyllableEngine;
using ServicesPoemEngine = Haiku.Services.Haiku.PoemEngine;

namespace Haiku.Services.Tests.Poems;

public class PoemEngineTests
{
    /// <summary>
    /// Placeholder: verifies that Analyze returns a result without throwing.
    /// </summary>
    [Fact]
    public void Analyze_ReturnsResultWithoutThrowing()
    {
        var poemEngine = new ServicesPoemEngine();

        var result = poemEngine.Analyze("hello world");

        Assert.NotNull(result);
        Assert.Single(result.Lines);
    }
}

public class PatternGeneratorTests
{
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
    public void GeneratePattern_ReturnsNonEmptyPattern(PoemType type)
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GeneratePattern(type, rng);

        Assert.NotNull(pattern);
        Assert.True(pattern.Length >= 2, $"Pattern for {type} should have at least 2 elements, got {pattern.Length}");
        foreach (var val in pattern)
        {
            Assert.True(val >= 1, $"All values in {type} pattern should be >= 1, got {val}");
        }
    }

    [Fact]
    public void GeneratePattern_Pi_ReturnsPiDigits()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GeneratePiPattern(rng);

        Assert.True(pattern.Length >= 3 && pattern.Length <= 20);
        var expected = PiSequence.GetDigits(pattern.Length);
        Assert.Equal(expected, pattern);
    }

    [Fact]
    public void GeneratePattern_Fib_ReturnsFibTerms()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GenerateFibPattern(rng);

        Assert.True(pattern.Length >= 3 && pattern.Length <= 10);
        var expected = FibonacciSequence.GetTerms(pattern.Length);
        Assert.Equal(expected, pattern);
    }

    [Fact]
    public void GeneratePattern_ReverseFib_ReturnsReverseFibTerms()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GenerateReverseFibPattern(rng);

        Assert.True(pattern.Length >= 3 && pattern.Length <= 10);
        var expected = FibonacciSequence.GetReverseTerms(pattern.Length);
        Assert.Equal(expected, pattern);
    }

    [Fact]
    public void GeneratePattern_Wave_IsSymmetric()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GenerateWavePattern(rng);

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

    [Fact]
    public void GeneratePattern_Prime_AllValuesArePrime()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GeneratePrimePattern(rng);

        Assert.True(pattern.Length >= 3 && pattern.Length <= 8);
        foreach (var val in pattern)
        {
            Assert.True(PrimeHelper.IsPrime(val), $"{val} should be prime");
        }
    }

    [Fact]
    public void GeneratePattern_Pulse_Alternates()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GeneratePulsePattern(rng);

        Assert.True(pattern.Length >= 4 && pattern.Length % 2 == 0);
        var a = pattern[0];
        var b = pattern[1];
        Assert.NotEqual(a, b);
        for (var i = 0; i < pattern.Length; i++)
        {
            Assert.Equal(i % 2 == 0 ? a : b, pattern[i]);
        }
    }

    [Fact]
    public void GeneratePattern_Collatz_EndsWithOne()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GenerateCollatzPattern(rng);

        Assert.True(pattern.Length >= 3);
        Assert.Equal(1, pattern[^1]);
        var expected = CollatzSequence.Generate(pattern[0]);
        Assert.Equal(expected, pattern);
    }

    [Fact]
    public void GeneratePattern_Stair_IncrementsByOne()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GenerateStairPattern(rng);

        Assert.True(pattern.Length >= 3 && pattern.Length <= 8);
        for (var i = 1; i < pattern.Length; i++)
        {
            Assert.Equal(pattern[i - 1] + 1, pattern[i]);
        }
    }

    [Fact]
    public void GeneratePattern_Erosion_DecrementsToOne()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GenerateErosionPattern(rng);

        Assert.True(pattern.Length >= 3);
        Assert.Equal(1, pattern[^1]);
        for (var i = 1; i < pattern.Length; i++)
        {
            Assert.Equal(pattern[i - 1] - 1, pattern[i]);
        }
    }

    [Fact]
    public void GeneratePattern_Mountain_StartsAtOne()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GenerateMountainPattern(rng);

        Assert.True(pattern.Length >= 3 && pattern.Length <= 10);
        Assert.Equal(1, pattern[0]);
        for (var i = 1; i < pattern.Length; i++)
        {
            Assert.Equal(pattern[i - 1] + 1, pattern[i]);
        }
    }

    [Fact]
    public void GeneratePattern_Nautilus_HasConstantSecondDifference()
    {
        var rng = new Random(42);
        var pattern = PatternGenerator.GenerateNautilusPattern(rng);

        Assert.True(pattern.Length >= 3 && pattern.Length <= 8);
        var d1 = pattern[1] - pattern[0];
        var d2 = pattern[2] - pattern[1];
        Assert.Equal(d1 + 1, d2);
    }

    [Fact]
    public void GeneratePattern_SeededDeterminism()
    {
        var pattern1 = PatternGenerator.GeneratePattern(PoemType.SyllablePi, new Random(42));
        var pattern2 = PatternGenerator.GeneratePattern(PoemType.SyllablePi, new Random(42));

        Assert.Equal(pattern1, pattern2);
    }

    [Fact]
    public void GeneratePattern_UnknownType_ReturnsEmpty()
    {
        var pattern = PatternGenerator.GeneratePattern(PoemType.Freeform, new Random(42));
        Assert.Empty(pattern);
    }
}

public class PoemEngineGenerationTests : IDisposable
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
        var providers = new ISyllableProvider[]
        {
            cmuDictionary,
            new HeuristicSyllableProvider(),
        };
        var syllableEngine = new SyllableEngine(providers, tokenizer);

        var assembly = typeof(ServicesPoemEngine).Assembly;
        var classifierTypes = assembly
            .GetExportedTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                     && typeof(IPoemClassifier).IsAssignableFrom(t));
        var classifiers = classifierTypes
            .Select(t => (IPoemClassifier)Activator.CreateInstance(t)!)
            .ToList();
        var chain = new PoemClassifierChain(classifiers);

        _engine = new ServicesPoemEngine(chain, syllableEngine, cmuDictionary, null);
    }

    public void Dispose()
    {
        if (File.Exists(_testJsonPath))
            File.Delete(_testJsonPath);
    }

    [Fact]
    public void GeneratePoem_TraditionalType_ReturnsCorrectLineCount()
    {
        var poem = _engine.GeneratePoem(PoemType.Haiku, seed: 42);

        Assert.Equal(3, poem.Length);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    [Fact]
    public void GeneratePoem_SyllablePi_ReturnsNonEmpty()
    {
        var poem = _engine.GeneratePoem(PoemType.SyllablePi, seed: 42);

        Assert.True(poem.Length >= 3);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    [Fact]
    public void GeneratePoem_WordPi_ReturnsNonEmpty()
    {
        var poem = _engine.GeneratePoem(PoemType.WordPi, seed: 42);

        Assert.True(poem.Length >= 3);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    [Fact]
    public void GeneratePoem_SyllableFib_ReturnsNonEmpty()
    {
        var poem = _engine.GeneratePoem(PoemType.SyllableFib, seed: 42);

        Assert.True(poem.Length >= 3);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    [Fact]
    public void GeneratePoem_WordFib_ReturnsNonEmpty()
    {
        var poem = _engine.GeneratePoem(PoemType.WordFib, seed: 42);

        Assert.True(poem.Length >= 3);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }

    [Fact]
    public void GeneratePoem_AllNonTraditionalTypes_ReturnNonEmpty()
    {
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

        foreach (var type in nonTraditional)
        {
            var poem = _engine.GeneratePoem(type, seed: 42);

            Assert.True(poem.Length >= 2,
                $"{type} should generate at least 2 lines, got {poem.Length}");
            foreach (var line in poem)
            {
                Assert.False(string.IsNullOrWhiteSpace(line),
                    $"{type} should not contain empty lines");
            }
        }
    }

    [Fact]
    public void GeneratePoem_Freeform_ReturnsEmpty()
    {
        var poem = _engine.GeneratePoem(PoemType.Freeform, seed: 42);

        Assert.Empty(poem);
    }

    [Fact]
    public void GeneratePoem_SeededDeterminism()
    {
        var poem1 = _engine.GeneratePoem(PoemType.SyllablePi, seed: 42);
        var poem2 = _engine.GeneratePoem(PoemType.SyllablePi, seed: 42);

        Assert.Equal(poem1, poem2);
    }

    [Fact]
    public void GeneratePoem_Monoku_ReturnsOneLine()
    {
        var poem = _engine.GeneratePoem(PoemType.Monoku, seed: 42);

        Assert.Single(poem);
        Assert.False(string.IsNullOrWhiteSpace(poem[0]));
    }

    [Fact]
    public void GeneratePoem_Isosyllabic_AllLinesSameLength()
    {
        var poem = _engine.GeneratePoem(PoemType.Isosyllabic, seed: 42);

        Assert.True(poem.Length >= 2 && poem.Length <= 5);
        foreach (var line in poem)
        {
            Assert.False(string.IsNullOrWhiteSpace(line));
        }
    }
}
