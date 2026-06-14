using Haiku.Domain.Enums;

namespace Haiku.Domain.ValueObjects;

/// <summary>Classification result for a poem, describing its type, syllable structure, and content.</summary>
public record PoemDefinition
{
    /// <summary>Gets the detected or assigned poem type.</summary>
    /// <value>One of the <see cref="PoemType"/> values (e.g. Haiku, Tanka, Sonnet).</value>
    public PoemType Type { get; init; }

    /// <summary>Gets the number of lines in the poem.</summary>
    /// <value>Derived from the content; must match the expected count for the detected <see cref="Type"/>.</value>
    public int LineCount { get; init; }

    /// <summary>Gets the syllable count for each line, in order.</summary>
    /// <value>An array whose length equals <see cref="LineCount"/>.</value>
    public int[] SyllablesPerLine { get; init; } = [];

    /// <summary>Gets the total number of syllables across all lines.</summary>
    /// <value>Sum of all elements in <see cref="SyllablesPerLine"/>.</value>
    public int TotalSyllableCount { get; init; }

    /// <summary>Gets the word count for each line, in order.</summary>
    /// <value>An array whose length equals <see cref="LineCount"/>.</value>
    public int[] WordCountPerLine { get; init; } = [];

    /// <summary>Gets the total number of words across all lines.</summary>
    /// <value>Sum of all elements in <see cref="WordCountPerLine"/>.</value>
    public int TotalWordCount { get; init; }

    /// <summary>Gets the original poem text as provided by the author.</summary>
    /// <value>May contain leading/trailing whitespace; use <see cref="NormalizedContent"/> for a cleaned form.</value>
    public string OriginalContent { get; init; } = string.Empty;

    /// <summary>Gets the normalized version of the poem (trimmed, lowercased, punctuation removed).</summary>
    /// <value>Derived from <see cref="OriginalContent"/>; used for syllable analysis and type detection.</value>
    public string NormalizedContent { get; init; } = string.Empty;

    /// <summary>Gets the optional theme detected or assigned to the poem.</summary>
    /// <value><see langword="null"/> when no theme has been assigned.</value>
    public string? Theme { get; init; }

    /// <summary>Gets optional extensible metadata associated with the poem.</summary>
    /// <value><see langword="null"/> when no metadata has been provided.</value>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>Gets the legacy display name for <see cref="Services.PoemEngine"/> compatibility.</summary>
    /// <value>Used by <see cref="Services.PoemEngine"/> until the Phase&#160;5 refactor replaces it with <see cref="Type"/>.</value>
    public string Name { get; init; } = string.Empty;

    /// <summary>Gets the legacy description for <see cref="Services.PoemEngine"/> compatibility.</summary>
    /// <value>Used by <see cref="Services.PoemEngine"/> until the Phase&#160;5 refactor replaces it with <see cref="Type"/>.</value>
    public string Description { get; init; } = string.Empty;

    /// <summary>Gets the legacy syllable pattern (e.g. [5,7,5] for haiku) for <see cref="Services.PoemEngine"/> compatibility.</summary>
    /// <value>Used by <see cref="Services.PoemEngine"/> until the Phase&#160;5 refactor replaces it with <see cref="SyllablesPerLine"/>.</value>
    public int[] SyllablePattern { get; init; } = [];
}
