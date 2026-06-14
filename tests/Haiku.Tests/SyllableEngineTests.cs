using Haiku.Services.Haiku;

namespace Haiku.Tests;

/// <summary>Unit tests for <see cref="SyllableEngine"/> covering syllable counting with custom dictionaries and punctuation stripping.</summary>
/// <remarks>
/// <para>
/// The syllable count resolution chain is: custom dictionary (highest priority) → CMU
/// pronunciation dictionary → heuristic fallback. Tests verify each tier and the priority
/// ordering between them. Counts default to 1 for unrecognized words that are non-empty.
/// </para>
/// </remarks>
public class SyllableEngineTests
{
    [Fact]
    public void CountWordSyllables_ReturnsCustomDictionaryCount_WhenWordExists()
    {
        // Custom dictionary has top priority; CMU dictionary and heuristic are only consulted
        // when a word is absent from the custom dict.
        var customDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "haiku", 2 } };
        var engine = new SyllableEngine(customDict);

        var result = engine.CountWordSyllables("haiku");

        Assert.Equal(2, result);
    }

    [Fact]
    public void CountWordSyllables_CustomDictionaryTakesPriority_OverCmuDictionary()
    {
        // Both dictionaries claim "hello" with different counts; custom (1) must win over CMU.
        var customDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "hello", 1 } };
        var cmuDict = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "HELLO" };
        var engine = new SyllableEngine(customDict, cmuDict);

        var result = engine.CountWordSyllables("hello");

        Assert.Equal(1, result);
    }

    [Fact]
    public void CountWordSyllables_UsesHeuristic_ForCmuKnownWords()
    {
        var cmuDict = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "SILENCE", "HELLO", "ELEPHANT" };
        var engine = new SyllableEngine(null, cmuDict);

        Assert.Equal(2, engine.CountWordSyllables("silence"));
        Assert.Equal(2, engine.CountWordSyllables("hello"));
        Assert.Equal(3, engine.CountWordSyllables("elephant"));
    }

    [Fact]
    public void CountWordSyllables_ReturnsAtLeastOne_ForUnknownWords()
    {
        var engine = new SyllableEngine();

        var result = engine.CountWordSyllables("xyzzy");

        Assert.InRange(result, 1, int.MaxValue);
    }

    [Fact]
    public void CountWordSyllables_ReturnsZero_ForBlankInput()
    {
        var engine = new SyllableEngine();

        Assert.Equal(0, engine.CountWordSyllables(""));
        Assert.Equal(0, engine.CountWordSyllables("   "));
    }

    [Fact]
    public void CountLineSyllables_ReturnsCountsForEachWord()
    {
        var customDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "the", 1 },
            { "silence", 2 },
            { "falls", 1 },
        };
        var engine = new SyllableEngine(customDict);

        var result = engine.CountLineSyllables("the silence falls");

        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { 1, 2, 1 }, result);
    }

    [Fact]
    public void CountLineSyllables_ReturnsEmpty_ForBlankLine()
    {
        var engine = new SyllableEngine();

        var result = engine.CountLineSyllables("");

        Assert.Empty(result);
    }

    [Fact]
    public void CountWordSyllables_StripsPunctuation()
    {
        var customDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "hello", 2 } };
        var engine = new SyllableEngine(customDict);

        Assert.Equal(2, engine.CountWordSyllables("hello!"));
        Assert.Equal(2, engine.CountWordSyllables("hello,"));
        Assert.Equal(2, engine.CountWordSyllables("\"hello\""));
    }

    [Fact]
    public void CountWordSyllables_HandlesCompoundWords()
    {
        var engine = new SyllableEngine();

        var result = engine.CountWordSyllables("well-known");

        Assert.InRange(result, 1, 10);
    }
}
