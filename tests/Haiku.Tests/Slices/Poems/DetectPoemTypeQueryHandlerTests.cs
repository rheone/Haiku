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

    #region Handle

    [Fact]
    public async Task Handle_ReturnsHaiku_For575_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("line one\nline two\nline three", new List<int> { 5, 7, 5 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Haiku, result);
    }

    [Fact]
    public async Task Handle_ReturnsTanka_For57577_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5", new List<int> { 5, 7, 5, 7, 7 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Tanka, result);
    }

    [Fact]
    public async Task Handle_ReturnsMonoku_ForSingleLineWithinRange_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("A single line of text", new List<int> { 7 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public async Task Handle_ReturnsMonoku_ForLowerBoundOf4Syllables_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(new DetectPoemTypeQuery("one line", new List<int> { 4 }), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public async Task Handle_ReturnsMonoku_ForUpperBoundOf17Syllables_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("long single line with many many words here", new List<int> { 17 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Monoku, result);
    }

    [Fact]
    public async Task Handle_DoesNotReturnMonoku_ForSingleLineBelow4Syllables_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(new DetectPoemTypeQuery("hi", new List<int> { 1 }), TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEqual(PoemType.Monoku, result);
    }

    [Fact]
    public async Task Handle_ReturnsFreeform_ForSingleLineAbove17Syllables_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery(
                "extra long single line well above the maximum monoku count of syllables",
                new List<int> { 18 }
            ),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Freeform, result);
    }

    [Fact]
    public async Task Handle_ReturnsAmericanLune_For353_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3", new List<int> { 3, 5, 3 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.AmericanLune, result);
    }

    [Fact]
    public async Task Handle_ReturnsKellyLune_For535_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3", new List<int> { 5, 3, 5 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.KellyLune, result);
    }

    [Fact]
    public async Task Handle_ReturnsCompressed_For232_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3", new List<int> { 2, 3, 2 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Compressed, result);
    }

    [Fact]
    public async Task Handle_ReturnsNearTraditional_For464_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3", new List<int> { 4, 6, 4 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.NearTraditional, result);
    }

    [Fact]
    public async Task Handle_ReturnsAmericanCinquain_For24682_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5", new List<int> { 2, 4, 6, 8, 2 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.AmericanCinquain, result);
    }

    [Fact]
    public async Task Handle_ReturnsReverseCinquain_For28642_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5", new List<int> { 2, 8, 6, 4, 2 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.ReverseCinquain, result);
    }

    [Fact]
    public async Task Handle_ReturnsSedoka_For577577_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6", new List<int> { 5, 7, 7, 5, 7, 7 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Sedoka, result);
    }

    [Fact]
    public async Task Handle_ReturnsButterflyCinquain_For246828642_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6\n7\n8\n9", new List<int> { 2, 4, 6, 8, 2, 8, 6, 4, 2 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.ButterflyCinquain, result);
    }

    [Fact]
    public async Task Handle_ReturnsMirrorCinquain_For2468228642_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6\n7\n8\n9\n10", new List<int> { 2, 4, 6, 8, 2, 2, 8, 6, 4, 2 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.MirrorCinquain, result);
    }

    [Fact]
    public async Task Handle_ReturnsChoka_ForMinimal7Lines_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6\n7", new List<int> { 5, 7, 5, 7, 5, 7, 7 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Choka, result);
    }

    [Fact]
    public async Task Handle_ReturnsIsosyllabic_For2LinesWithEqualSyllables_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("line one\nline two", new List<int> { 5, 5 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Isosyllabic, result);
    }

    [Fact]
    public async Task Handle_DoesNotReturnIsosyllabic_ForSingleLine_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(new DetectPoemTypeQuery("single line", new List<int> { 5 }), TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEqual(PoemType.Isosyllabic, result);
    }

    [Fact]
    public async Task Handle_ReturnsFreeform_WhenNoPatternMatches_Test()
    {
        // Arrange
        var handler = new DetectPoemTypeQueryHandler(_poemEngine);

        // Act
        var result = await handler.Handle(
            new DetectPoemTypeQuery("line one\nline two", new List<int> { 3, 9 }),
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.Equal(PoemType.Freeform, result);
    }

    #endregion
}
