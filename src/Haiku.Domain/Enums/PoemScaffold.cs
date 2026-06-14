namespace Haiku.Domain.Enums;

/// <summary>
/// The structural measurement axis a poem type uses.
/// Inferred automatically by the classifier that matches.
/// </summary>
public enum PoemScaffold
{
    /// <summary>Structure is measured in syllables per line.</summary>
    SyllableBased,

    /// <summary>Structure is measured in words per line.</summary>
    WordBased,
}
