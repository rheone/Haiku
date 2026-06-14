using Haiku.Domain.Enums;
using Haiku.Services.Poems.Classifiers;
using Haiku.Services.Slices.Poems;
using Haiku.Services.Syllables;
using Haiku.Services.Syllables.Providers;

namespace Haiku.Tests.Slices.Poems;

/// <summary>Unit tests for <see cref="DetectPoemTypeQueryHandler"/> covering all classifier types in the chain.</summary>
public class DetectPoemTypeQueryHandlerTests
{
    // Chain and Handler are static and shared across tests. Both are stateless
    // (no mutable fields), so this is thread-safe for parallel test execution.
    private static readonly PoemClassifierChain Chain = new(
        new IPoemClassifier[]
        {
            new MonokuClassifier(),
            new HaikuClassifier(),
            new KatautaClassifier(),
            new AmericanLuneClassifier(),
            new KellyLuneClassifier(),
            new CompressedClassifier(),
            new NearTraditionalClassifier(),
            new TankaClassifier(),
            new AmericanCinquainClassifier(),
            new ReverseCinquainClassifier(),
            new SedokaClassifier(),
            new ButterflyCinquainClassifier(),
            new MirrorCinquainClassifier(),
            new ChokaClassifier(),
            new IsosyllabicClassifier(),
            new FreeformClassifier(),
        }
    );

    private static readonly DetectPoemTypeQueryHandler Handler = new(
        Chain,
        new SyllableEngine([new HeuristicSyllableProvider()], new WordTokenizer()),
        new WordTokenizer()
    );

    #region Haiku

    /// <summary>Verifies that a 5-7-5 syllable pattern is classified as Haiku.</summary>
    [Fact]
    public async Task Handle_ReturnsHaiku_For575_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("line one\nline two\nline three", new List<int> { 5, 7, 5 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Haiku, result);
    }

    #endregion

    #region Tanka

    /// <summary>Verifies that a 5-7-5-7-7 syllable pattern is classified as Tanka.</summary>
    [Fact]
    public async Task Handle_ReturnsTanka_For57577_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3\n4\n5", new List<int> { 5, 7, 5, 7, 7 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Tanka, result);
    }

    #endregion

    #region Monoku

    /// <summary>Verifies that a single line within the monoku syllable range returns Monoku.</summary>
    [Fact]
    public async Task Handle_ReturnsMonoku_ForSingleLineWithinRange_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("A single line of text", new List<int> { 7 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Monoku, result);
    }

    /// <summary>Verifies that a single line at the lower bound (4 syllables) returns Monoku.</summary>
    [Fact]
    public async Task Handle_ReturnsMonoku_ForLowerBoundOf4Syllables_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("one line", new List<int> { 4 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Monoku, result);
    }

    /// <summary>Verifies that a single line at the upper bound (17 syllables) returns Monoku.</summary>
    [Fact]
    public async Task Handle_ReturnsMonoku_ForUpperBoundOf17Syllables_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("long single line with many many words here", new List<int> { 17 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Monoku, result);
    }

    /// <summary>Verifies that a single line below 4 syllables does not return Monoku.</summary>
    [Fact]
    public async Task Handle_DoesNotReturnMonoku_ForSingleLineBelow4Syllables_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("hi", new List<int> { 1 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEqual(PoemType.Monoku, result);
    }

    #endregion

    #region Katauta

    /// <summary>Verifies that a 5-7-7 syllable pattern is classified as Katauta.</summary>
    [Fact]
    public async Task Handle_ReturnsKatauta_For577_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3", new List<int> { 5, 7, 7 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Katauta, result);
    }

    #endregion

    #region American Lune, Kelly Lune, Compressed, Near Traditional

    /// <summary>Verifies that a 3-5-3 syllable pattern is classified as American Lune.</summary>
    [Fact]
    public async Task Handle_ReturnsAmericanLune_For353_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3", new List<int> { 3, 5, 3 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.AmericanLune, result);
    }

    /// <summary>Verifies that a 5-3-5 syllable pattern is classified as Kelly Lune.</summary>
    [Fact]
    public async Task Handle_ReturnsKellyLune_For535_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3", new List<int> { 5, 3, 5 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.KellyLune, result);
    }

    /// <summary>Verifies that a 2-3-2 syllable pattern is classified as Compressed.</summary>
    [Fact]
    public async Task Handle_ReturnsCompressed_For232_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3", new List<int> { 2, 3, 2 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Compressed, result);
    }

    /// <summary>Verifies that a 4-6-4 syllable pattern is classified as Near Traditional.</summary>
    [Fact]
    public async Task Handle_ReturnsNearTraditional_For464_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3", new List<int> { 4, 6, 4 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.NearTraditional, result);
    }

    #endregion

    #region Cinquain variants

    /// <summary>Verifies that a 2-4-6-8-2 syllable pattern is classified as American Cinquain.</summary>
    [Fact]
    public async Task Handle_ReturnsAmericanCinquain_For24682_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3\n4\n5", new List<int> { 2, 4, 6, 8, 2 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.AmericanCinquain, result);
    }

    /// <summary>Verifies that a 2-8-6-4-2 syllable pattern is classified as Reverse Cinquain.</summary>
    [Fact]
    public async Task Handle_ReturnsReverseCinquain_For28642_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3\n4\n5", new List<int> { 2, 8, 6, 4, 2 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.ReverseCinquain, result);
    }

    #endregion

    #region Sedoka

    /// <summary>Verifies that a 5-7-7-5-7-7 syllable pattern is classified as Sedoka.</summary>
    [Fact]
    public async Task Handle_ReturnsSedoka_For577577_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6", new List<int> { 5, 7, 7, 5, 7, 7 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Sedoka, result);
    }

    #endregion

    #region Butterfly and Mirror Cinquain

    /// <summary>Verifies that a 2-4-6-8-2-8-6-4-2 syllable pattern is classified as Butterfly Cinquain.</summary>
    [Fact]
    public async Task Handle_ReturnsButterflyCinquain_For246828642_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6\n7\n8\n9", new List<int> { 2, 4, 6, 8, 2, 8, 6, 4, 2 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.ButterflyCinquain, result);
    }

    /// <summary>Verifies that a 2-4-6-8-2-2-8-6-4-2 syllable pattern is classified as Mirror Cinquain.</summary>
    [Fact]
    public async Task Handle_ReturnsMirrorCinquain_For2468228642_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6\n7\n8\n9\n10", new List<int> { 2, 4, 6, 8, 2, 2, 8, 6, 4, 2 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.MirrorCinquain, result);
    }

    #endregion

    #region Choka

    /// <summary>Verifies that a 7-line alternating 5-7-5-7-5-7-7 syllable pattern is classified as Choka.</summary>
    [Fact]
    public async Task Handle_ReturnsChoka_ForMinimal7Lines_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("1\n2\n3\n4\n5\n6\n7", new List<int> { 5, 7, 5, 7, 5, 7, 7 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Choka, result);
    }

    #endregion

    #region Isosyllabic

    /// <summary>Verifies that two lines with equal syllable counts are classified as Isosyllabic.</summary>
    [Fact]
    public async Task Handle_ReturnsIsosyllabic_For2LinesWithEqualSyllables_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("line one\nline two", new List<int> { 5, 5 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Isosyllabic, result);
    }

    /// <summary>Verifies that a single line is not classified as Isosyllabic.</summary>
    [Fact]
    public async Task Handle_DoesNotReturnIsosyllabic_ForSingleLine_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("single line", new List<int> { 5 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEqual(PoemType.Isosyllabic, result);
    }

    #endregion

    #region Freeform and edge cases

    /// <summary>Verifies that a single line above 17 syllables is classified as Freeform.</summary>
    [Fact]
    public async Task Handle_ReturnsFreeform_ForSingleLineAbove17Syllables_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery(
            "extra long single line well above the maximum monoku count of syllables",
            new List<int> { 18 }
        );

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Freeform, result);
    }

    /// <summary>Verifies that when no classifier matches, the result is Freeform.</summary>
    [Fact]
    public async Task Handle_ReturnsFreeform_WhenNoPatternMatches_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("line one\nline two", new List<int> { 3, 9 });

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Freeform, result);
    }

    /// <summary>Verifies that an empty content string is classified as Freeform.</summary>
    /// <remarks>Auto Generated, verify expected behavior:</remarks>
    [Fact]
    public async Task Handle_ReturnsFreeform_ForEmptyContent_Test()
    {
        // Arrange
        var query = new DetectPoemTypeQuery("");

        // Act
        var result = await Handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(PoemType.Freeform, result);
    }

    #endregion

    #region Cancellation

    /// <summary>Verifies that <see cref="OperationCanceledException"/> is thrown when the cancellation token is cancelled.</summary>
    /// <remarks>Auto Generated, verify expected behavior:</remarks>
    [Fact]
    public async Task Handle_ThrowsOperationCanceledException_WhenCancelled_Test()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            Handler.Handle(
                new DetectPoemTypeQuery("line one\nline two\nline three", new List<int> { 5, 7, 5 }),
                cts.Token
            )
        );
    }

    #endregion
}
