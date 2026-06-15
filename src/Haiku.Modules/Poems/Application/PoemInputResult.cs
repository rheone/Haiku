namespace Haiku.Modules.Poems.Application;

/// <summary>Contains the normalized result of processing raw poem input through <see cref="IPoemInputService"/>.</summary>
public record PoemInputResult
{
    /// <summary>Gets the poem content after normalizing whitespace and stripping zero-width characters.</summary>
    /// <value>The normalized poem text.</value>
    public string NormalizedContent { get; init; } = string.Empty;

    /// <summary>Gets the individual non-empty lines of the poem.</summary>
    /// <value>An array of line strings.</value>
    public string[] Lines { get; init; } = [];

    /// <summary>Gets the syllable count for each line, in order.</summary>
    /// <value>An array of syllable counts per line.</value>
    public int[] LineSyllableCounts { get; init; } = [];

    /// <summary>Gets the total syllable count across all lines.</summary>
    /// <value>The sum of all line syllable counts.</value>
    public int TotalSyllables { get; init; }

    /// <summary>Gets the list of validation errors, if any.</summary>
    /// <value>A read-only list of error messages.</value>
    public IReadOnlyList<string> Errors { get; init; } = [];

    /// <summary>Gets a value indicating whether the input is valid, meaning no validation errors were found.</summary>
    /// <value><c>true</c> if <see cref="Errors"/> is empty; otherwise <c>false</c>.</value>
    public bool IsValid => Errors.Count == 0;

    /// <summary>Gets the detected poem type, or <c>null</c> if detection was not performed.</summary>
    /// <value>The detected <see cref="PoemType"/>, or <c>null</c>.</value>
    public PoemType? DetectedType { get; init; }

    /// <summary>Gets the full poem classification result, or <c>null</c> if detection was not performed.</summary>
    /// <value>The full poem classification result with display metadata, or <c>null</c> if detection was not performed.</value>
    public PoemDefinition? PoemDefinition { get; init; }
}
