namespace Haiku.Services.Syllables;

public record TokenizedLine
{
    public string[] Words { get; init; } = [];
    public int[] WordSyllableCounts { get; init; } = [];
    public int TotalSyllables { get; init; }
    public int WordCount { get; init; }
}
