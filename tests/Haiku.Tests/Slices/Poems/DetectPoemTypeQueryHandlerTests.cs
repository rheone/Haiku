using Haiku.Domain.Enums;
using Haiku.Services.Haiku;
using Haiku.Services.Slices.Poems;

namespace Haiku.Tests.Slices.Poems;

/// <summary>Unit tests for <see cref="DetectPoemTypeQueryHandler"/> covering all 15 poem type detections.</summary>
/// <remarks>
/// <para>
/// Each test supplies a syllable-per-line vector to the handler, which delegates to
/// <see cref="PoemService.DetectPoemType"/> and ultimately the <c>PoemMatcherChain</c>.
/// The chain tests patterns in priority order: haiku (5-7-5), tanka (5-7-5-7-7),
/// monoku (1 line, 4-17 syllables), katauta (5-7-7), American lune (3-5-3),
/// Kelly lune (5-3-5), compressed (2-3-2), near-traditional (4-6-4),
/// American cinquain (2-4-6-8-2), reverse cinquain (2-8-6-4-2), sedoka (5-7-7-5-7-7),
/// butterfly cinquain (2-4-6-8-2-8-6-4-2), mirror cinquain (2-4-6-8-2-2-8-6-4-2),
/// choka (7+ lines alternating 5-7 ending 7-7), isosyllabic (2+ lines equal count),
/// and freeform (catch-all when nothing else matches).
/// </para>
/// </remarks>
public class DetectPoemTypeQueryHandlerTests
{
    private readonly PoemEngine _poemEngine = new();

    [Fact]
    public async Task Handle_ReturnsHaiku_For575()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("line one\nline two\nline three", new List<int> { 5, 7, 5 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Haiku, result);
    }

    [Fact]
    public async Task Handle_ReturnsTanka_For57577()
    {
        // Tanka extends haiku with two additional 7-syllable lines (5-7-5-7-7).
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5", new List<int> { 5, 7, 5, 7, 7 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Tanka, result);
    }

    [Fact]
    public async Task Handle_ReturnsMonoku_ForSingleLineWithinRange()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("A single line of text", new List<int> { 7 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public async Task Handle_ReturnsMonoku_ForLowerBoundOf4Syllables()
    {
        // Monoku boundary: single lines with 4-17 syllables qualify. Below 4 is freeform.
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(new DetectPoemTypeQuery("one line", new List<int> { 4 }), CancellationToken.None);

        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public async Task Handle_ReturnsMonoku_ForUpperBoundOf17Syllables()
    {
        // Monoku boundary: single lines with 4-17 syllables qualify. Above 17 is freeform.
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("long single line with many many words here", new List<int> { 17 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public async Task Handle_DoesNotReturnMonoku_ForSingleLineBelow4Syllables()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(new DetectPoemTypeQuery("hi", new List<int> { 1 }), CancellationToken.None);

        Assert.NotEqual(PoemType.Monoku, result);
    }

    [Fact]
    public async Task Handle_ReturnsFreeform_ForSingleLineAbove17Syllables()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery(
                "extra long single line well above the maximum monoku count of syllables",
                new List<int> { 18 }
            ),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Freeform, result);
    }

    [Fact]
    public async Task Handle_ReturnsAmericanLune_For353()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3", new List<int> { 3, 5, 3 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.AmericanLune, result);
    }

    [Fact]
    public async Task Handle_ReturnsKellyLune_For535()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3", new List<int> { 5, 3, 5 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.KellyLune, result);
    }

    [Fact]
    public async Task Handle_ReturnsCompressed_For232()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3", new List<int> { 2, 3, 2 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Compressed, result);
    }

    [Fact]
    public async Task Handle_ReturnsNearTraditional_For464()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3", new List<int> { 4, 6, 4 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.NearTraditional, result);
    }

    [Fact]
    public async Task Handle_ReturnsAmericanCinquain_For24682()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5", new List<int> { 2, 4, 6, 8, 2 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.AmericanCinquain, result);
    }

    [Fact]
    public async Task Handle_ReturnsReverseCinquain_For28642()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5", new List<int> { 2, 8, 6, 4, 2 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.ReverseCinquain, result);
    }

    [Fact]
    public async Task Handle_ReturnsSedoka_For577577()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6", new List<int> { 5, 7, 7, 5, 7, 7 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Sedoka, result);
    }

    [Fact]
    public async Task Handle_ReturnsButterflyCinquain_For246828642()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6\n7\n8\n9", new List<int> { 2, 4, 6, 8, 2, 8, 6, 4, 2 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.ButterflyCinquain, result);
    }

    [Fact]
    public async Task Handle_ReturnsMirrorCinquain_For2468228642()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6\n7\n8\n9\n10", new List<int> { 2, 4, 6, 8, 2, 2, 8, 6, 4, 2 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.MirrorCinquain, result);
    }

    [Fact]
    public async Task Handle_ReturnsChoka_ForMinimal7Lines()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6\n7", new List<int> { 5, 7, 5, 7, 5, 7, 7 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Choka, result);
    }

    [Fact]
    public async Task Handle_ReturnsIsosyllabic_For2LinesWithEqualSyllables()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("line one\nline two", new List<int> { 5, 5 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Isosyllabic, result);
    }

    [Fact]
    public async Task Handle_DoesNotReturnIsosyllabic_ForSingleLine()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(new DetectPoemTypeQuery("single line", new List<int> { 5 }), CancellationToken.None);

        Assert.NotEqual(PoemType.Isosyllabic, result);
    }

    [Fact]
    public async Task Handle_ReturnsFreeform_WhenNoPatternMatches()
    {
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);
        var result = await handler.Handle(
            new DetectPoemTypeQuery("line one\nline two", new List<int> { 3, 9 }),
            CancellationToken.None
        );

        Assert.Equal(PoemType.Freeform, result);
    }
}
