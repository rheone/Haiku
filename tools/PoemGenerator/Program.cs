using System.Reflection;
using System.Text;
using Haiku.Modules;
using Haiku.Modules.Poems.Application;
using Haiku.Modules.Poems.Classifiers;
using Haiku.Modules.Poems.Classifiers.SequenceHelpers;
using Haiku.Modules.Poems.Rhyming;
using Haiku.Modules.Poems.Syllables;
using Haiku.Modules.Poems.Syllables.Providers;
using Haiku.Modules.Shared.Domain.Enums;
using Haiku.Modules.Shared.Domain.ValueObjects;
using Spectre.Console;

// ─────────────────────────────────────────────────────────────
// Engine setup
// ─────────────────────────────────────────────────────────────

var cmuDictPath = Path.Combine(AppContext.BaseDirectory, "Resources", "cmudict.json");
if (!File.Exists(cmuDictPath))
{
    AnsiConsole.MarkupLine("[red]CMU dictionary not found at [/][yellow]{0}[/]", cmuDictPath);
    AnsiConsole.MarkupLine("Run [yellow]dotnet run tools/build-cmudict.cs[/] first, or set [yellow]CMUDICT_OUTPUT[/].");
    return 1;
}

var cmuDictionary = new CmuDictionaryProvider(cmuDictPath);
var tokenizer = new WordTokenizer();
var providers = new ISyllableProvider[] { cmuDictionary, new HeuristicSyllableProvider() };
var syllableEngine = new SyllableEngine(providers, tokenizer);

var assembly = typeof(PoemEngine).Assembly;
var classifierTypes = assembly
    .GetExportedTypes()
    .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IPoemClassifier).IsAssignableFrom(t));
var classifiers = classifierTypes.Select(t => (IPoemClassifier)Activator.CreateInstance(t)!).ToList();
var chain = new PoemClassifierChain(classifiers);

var poemEngine = new PoemEngine(chain, syllableEngine, cmuDictionary, null);

var allDefs = poemEngine.GetAllDefinitions().OrderBy(d => d.DisplayName).ToList();
var allTypes = allDefs.Select(d => d.Type).Where(t => t != PoemType.Freeform).OrderBy(t => t.ToString()).ToList();

// Get commit hash at startup
var commitHash = GetCommitHash();

// Output directory — relative to repo root
var repoRoot = FindGitDir(Environment.CurrentDirectory) is string gd
    ? Path.GetDirectoryName(gd) ?? Environment.CurrentDirectory
    : Environment.CurrentDirectory;
var outputDir = Path.Combine(repoRoot, "generated-poems");
Directory.CreateDirectory(outputDir);

// ─────────────────────────────────────────────────────────────
// Main TUI loop
// ─────────────────────────────────────────────────────────────

AnsiConsole.Write(new FigletText("Poem Generator").Centered().Color(Color.Purple));
AnsiConsole.MarkupLine("[grey]Interactive poem generation using the Haiku engine[/]");
AnsiConsole.WriteLine();

while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold]Main Menu[/]")
            .PageSize(10)
            .AddChoices("Generate Poems", "Help / About", "Exit")
    );

    if (choice == "Exit")
        break;

    if (choice == "Help / About")
    {
        ShowHelp(allDefs, cmuDictionary);
        continue;
    }

    // ── Generate flow ──────────────────────────────────────
    var (selectedTypes, poemsPerType) = SelectTypes(allTypes, allDefs);
    if (selectedTypes.Count == 0)
        continue;

    var seed = AnsiConsole.Prompt(
        new TextPrompt<string>("[grey]Random seed[/] (press Enter for random):").AllowEmpty().DefaultValue("")
    );

    int? seedValue = string.IsNullOrWhiteSpace(seed) ? null : int.Parse(seed);

    var outputMode = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold]Output[/]")
            .AddChoices("Display in terminal", "Save to markdown file", "Both")
    );

    // ── Generate ───────────────────────────────────────────
    var results = new List<(PoemType Type, PoemDefinition Def, string[] Lines)>();
    var failures = new List<GenerationFailure>();

    await AnsiConsole
        .Status()
        .Spinner(Spinner.Known.Dots)
        .SpinnerStyle(Style.Parse("purple"))
        .StartAsync(
            "Generating poems...",
            async _ =>
            {
                var rng = seedValue.HasValue ? new Random(seedValue.Value) : Random.Shared;
                var perType = poemsPerType;

                foreach (var poemType in selectedTypes)
                {
                    var def = poemEngine.GetDefinition(poemType);
                    for (var i = 0; i < perType; i++)
                    {
                        var poemSeed = rng.Next();
                        try
                        {
                            var lines = poemEngine.GeneratePoem(poemType, poemSeed);
                            results.Add((poemType, def, lines));
                        }
                        catch (Exception ex)
                        {
                            failures.Add(
                                new GenerationFailure(
                                    def.DisplayName,
                                    poemType,
                                    i + 1,
                                    seedValue,
                                    poemSeed,
                                    ex.GetType().FullName ?? ex.GetType().Name,
                                    ex.Message,
                                    ex.StackTrace ?? "(no stack trace)"
                                )
                            );
                        }
                        finally
                        {
                            // Tiny yield to keep UI responsive
                            await Task.Delay(1);
                        }
                    }
                }
            }
        );

    if (results.Count == 0 && failures.Count == 0)
    {
        AnsiConsole.MarkupLine("[red]No poems were generated.[/]");
        AnsiConsole.WriteLine();
        continue;
    }

    // ── Display ────────────────────────────────────────────
    if (outputMode is "Display in terminal" or "Both")
    {
        DisplayPoems(results, failures, seedValue);
    }

    // ── Save ───────────────────────────────────────────────
    if (outputMode is "Save to markdown file" or "Both")
    {
        var fileName = $"PoemGenerator_{DateTime.Now:yyyyMMdd-HHmmss}_{commitHash}.md";
        var filePath = Path.Combine(outputDir, fileName);

        var markdown = BuildMarkdown(results, failures, seedValue, commitHash);

        await File.WriteAllTextAsync(filePath, markdown);

        AnsiConsole.MarkupLine("[green]Saved:[/] [yellow]{0}[/]", filePath);
    }

    AnsiConsole.WriteLine();
}

return 0;

// ─────────────────────────────────────────────────────────────
// Local functions
// ─────────────────────────────────────────────────────────────

static string GetCommitHash()
{
    try
    {
        var gitDir = FindGitDir(Environment.CurrentDirectory);
        if (gitDir is null)
            return "unknown";

        var headFile = Path.Combine(gitDir, "HEAD");
        if (!File.Exists(headFile))
            return "unknown";

        var head = File.ReadAllText(headFile).Trim();
        if (head.StartsWith("ref: "))
        {
            var refPath = Path.Combine(gitDir, head[5..]);
            return File.Exists(refPath) ? File.ReadAllText(refPath).Trim() : "unknown";
        }

        return head;
    }
    catch
    {
        return "unknown";
    }
}

static string? FindGitDir(string? dir)
{
    while (dir is not null)
    {
        var gitDir = Path.Combine(dir, ".git");
        if (Directory.Exists(gitDir))
            return gitDir;
        dir = Path.GetDirectoryName(dir);
    }

    return null;
}

(List<PoemType> SelectedTypes, int PoemsPerType) SelectTypes(List<PoemType> allTypes, List<PoemDefinition> allDefs)
{
    var method = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[bold]How to select poem types?[/]")
            .PageSize(10)
            .AddChoices($"All types ({allTypes.Count})", "Pick specific types", "Random subset")
    );

    List<PoemType> selected;

    var allTypesLabel = $"All types ({allTypes.Count})";
    switch (method)
    {
        case var _ when method == allTypesLabel:
            selected = allTypes;
            break;

        case "Pick specific types":
            selected = PickSpecificTypes(allDefs);
            break;

        case "Random subset":
            var count = AnsiConsole.Prompt(
                new TextPrompt<int>("[grey]How many random types?[/]")
                    .DefaultValue(5)
                    .Validate(c =>
                        c >= 1 && c <= allTypes.Count
                            ? ValidationResult.Success()
                            : ValidationResult.Error($"1-{allTypes.Count}")
                    )
            );
            var rng = Random.Shared;
            selected = [.. allTypes.OrderBy(_ => rng.Next()).Take(count)];
            break;

        default:
            selected = [];
            break;
    }

    if (selected.Count == 0)
        return (selected, 0);

    var perType = AnsiConsole.Prompt(
        new TextPrompt<int>("[grey]Poems per type[/]")
            .DefaultValue(3)
            .Validate(p => p >= 1 ? ValidationResult.Success() : ValidationResult.Error("Min 1"))
    );

    return (selected, perType);
}

List<PoemType> PickSpecificTypes(List<PoemDefinition> allDefs)
{
    var selectable = allDefs.Where(d => d.Type != PoemType.Freeform).OrderBy(d => d.DisplayName).ToList();

    var selected = AnsiConsole.Prompt(
        new MultiSelectionPrompt<string>()
            .Title("[bold]Pick poem types[/]")
            .PageSize(20)
            .InstructionsText("[grey](space to toggle, enter to confirm)[/]")
            .AddChoices(selectable.Select(d => d.DisplayName))
    );

    return allDefs.Where(d => selected.Contains(d.DisplayName)).Select(d => d.Type).ToList();
}

void DisplayPoems(
    List<(PoemType Type, PoemDefinition Def, string[] Lines)> results,
    List<GenerationFailure> failures,
    int? seed
)
{
    foreach (var group in results.GroupBy(r => r.Type))
    {
        var def = group.First().Def;
        var grid = new Grid();
        grid.AddColumn(new GridColumn().NoWrap());
        grid.AddColumn();

        var index = 0;
        foreach (var (_, _, lines) in group)
        {
            index++;
            var poemPanel = new Panel(
                Align.Left(new Markup(string.Join("\n", lines.Select(l => $"[white]{Markup.Escape(l)}[/]"))))
            )
            {
                Header = new PanelHeader($"#{index}"),
                Padding = new Padding(2, 0, 2, 1),
                BorderStyle = new Style(Color.Purple),
            };

            var scaffold = def.Scaffold;
            var counts =
                scaffold == Haiku.Modules.Shared.Domain.Enums.PoemScaffold.WordBased
                    ? lines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length).ToArray()
                    : lines.Select(l => poemEngine.CountLineSyllables(l)).ToArray();
            var summary =
                lines.Length > 6
                    ? $"[grey]{lines.Length} lines[/]"
                    : $"[grey]{string.Join("-", counts)} {(scaffold == Haiku.Modules.Shared.Domain.Enums.PoemScaffold.WordBased ? "words" : "syllables")}[/]";

            grid.AddRow(new Markup($"[bold]{def.DisplayName}[/]\n{summary}"), poemPanel);
        }

        AnsiConsole.Write(new Padder(grid, new Padding(0, 1)));
    }

    if (failures.Count > 0)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold red]Generation Failures[/]");
        AnsiConsole.WriteLine();

        foreach (var f in failures)
        {
            var detail =
                $"Type: {Markup.Escape(f.DisplayName)}\n"
                + $"Attempt: #{f.AttemptNumber}\n"
                + $"Exception: {Markup.Escape(f.ExceptionType)}: {Markup.Escape(f.Message)}\n"
                + $"Global seed: {f.GlobalSeed?.ToString() ?? "(random)"}\n"
                + $"Stack trace:\n{Markup.Escape(f.StackTrace)}";

            var panel = new Panel(new Markup($"[red]{detail}[/]"))
            {
                Header = new PanelHeader($"[bold]FAILED[/] {Markup.Escape(f.DisplayName)} #{f.AttemptNumber}"),
                BorderStyle = new Style(Color.Red),
                Padding = new Padding(2, 1),
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }
    }

    if (seed.HasValue)
    {
        AnsiConsole.MarkupLine("[grey]Seed: [/][yellow]{0}[/]", seed.Value);
    }
}

string BuildMarkdown(
    List<(PoemType Type, PoemDefinition Def, string[] Lines)> results,
    List<GenerationFailure> failures,
    int? seed,
    string commitHash
)
{
    var sb = new StringBuilder();
    var now = DateTime.Now;
    var utcNow = DateTime.UtcNow;

    sb.AppendLine("# Generated Poems");
    sb.AppendLine();
    sb.AppendLine($"- **Generated:** {now:yyyy-MM-dd HH:mm:ss} local ({utcNow:yyyy-MM-dd HH:mm:ss} UTC)");
    sb.AppendLine($"- **Commit:** {commitHash}");
    if (seed.HasValue)
        sb.AppendLine($"- **Seed:** {seed.Value}");
    var successCount = results.Count;
    var failCount = failures.Count;
    sb.AppendLine(
        $"- **Poems:** {successCount + failCount} total"
            + $" ({successCount} succeeded, {failCount} failed)"
            + $", {results.Select(r => r.Type).Distinct().Count()} types"
    );
    sb.AppendLine();

    foreach (var group in results.GroupBy(r => r.Type))
    {
        var def = group.First().Def;
        var pattern = def.SyllablePattern is not null ? string.Join("-", def.SyllablePattern) : "(see description)";

        sb.AppendLine($"## {def.DisplayName}");
        sb.AppendLine();
        sb.AppendLine($"*{def.Description}*");
        sb.AppendLine();

        if (def.SyllablePattern is not null)
        {
            sb.AppendLine($"**Expected pattern:** {pattern} ({def.SyllablePattern.Length} lines)");
            sb.AppendLine();
        }

        foreach (var (_, _, lines) in group)
        {
            foreach (var line in lines)
            {
                sb.AppendLine($"> {line}");
            }

            var isWordBased = def.Scaffold == Haiku.Modules.Shared.Domain.Enums.PoemScaffold.WordBased;
            var counts = isWordBased
                ? lines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length).ToArray()
                : lines.Select(l => poemEngine.CountLineSyllables(l)).ToArray();
            var actualPattern = string.Join("-", counts);
            var total = counts.Sum();
            var unitLabel = isWordBased ? "words" : "syllables";

            sb.AppendLine();
            sb.AppendLine(
                $"- **Type:** {def.DisplayName} | **Lines:** {lines.Length} | "
                    + $"**Total {unitLabel}:** {total} | **Pattern:** {actualPattern}"
            );
            sb.AppendLine();
        }
    }

    if (failures.Count > 0)
    {
        sb.AppendLine();
        sb.AppendLine("## Generation Errors");
        sb.AppendLine();

        foreach (var f in failures)
        {
            sb.AppendLine("### " + f.DisplayName + " #" + f.AttemptNumber);
            sb.AppendLine();
            sb.AppendLine("| Field | Value |");
            sb.AppendLine("|-------|-------|");
            sb.AppendLine($"| **Type** | {f.DisplayName} |");
            sb.AppendLine($"| **Attempt** | #{f.AttemptNumber} |");
            sb.AppendLine($"| **Global seed** | {f.GlobalSeed?.ToString() ?? "(random)"} |");
            sb.AppendLine($"| **Exception** | `{f.ExceptionType}` |");
            sb.AppendLine($"| **Message** | {f.Message} |");
            sb.AppendLine();
            sb.AppendLine("**Stack trace:**");
            sb.AppendLine();
            sb.AppendLine("```");
            sb.AppendLine(f.StackTrace);
            sb.AppendLine("```");
            sb.AppendLine();
        }
    }

    return sb.ToString();
}

void ShowHelp(List<PoemDefinition> allDefs, CmuDictionaryProvider cmuDict)
{
    AnsiConsole.WriteLine();
    AnsiConsole.Write(
        new Panel(
            new Markup(
                "Generates random poems using the [bold]Haiku[/] engine's CMU pronunciation "
                    + $"dictionary ([yellow]{cmuDict.GetType().Name}[/], 126K words).\n\n"
                    + $"All [yellow]{allDefs.Count(d => d.Type != PoemType.Freeform)}[/] poem types are supported, "
                    + "including traditional syllable-based\n"
                    + "forms (Haiku, Tanka, etc.) and non-traditional types (Pi, Fib, Wave, Prime,\n"
                    + "Hailstone, etc.) in both syllable-based and word-based variants.\n\n"
                    + "Use the interactive prompts to select types, set counts, and choose output."
            )
        )
        {
            Header = new PanelHeader("About"),
            Padding = new Padding(2, 1),
        }
    );
    AnsiConsole.WriteLine();

    var table = new Table()
        .Border(TableBorder.Rounded)
        .AddColumn("Type")
        .AddColumn("Category")
        .AddColumn("Lines")
        .AddColumn("Pattern");

    foreach (var def in allDefs.Where(d => d.Type != PoemType.Freeform).OrderBy(d => d.DisplayName))
    {
        if (def.SyllablePattern is not null)
        {
            table.AddRow(
                def.DisplayName,
                def.Category.ToString(),
                def.SyllablePattern.Length.ToString(),
                string.Join("-", def.SyllablePattern)
            );
        }
        else
        {
            var unitLabel = def.Scaffold == Haiku.Modules.Shared.Domain.Enums.PoemScaffold.WordBased ? "words" : "syllables";
            table.AddRow(def.DisplayName, def.Category.ToString(), "?", $"(sequence, per-line {unitLabel})");
        }
    }

    AnsiConsole.Write(table);
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[grey]Press any key to return to menu...[/]");
    Console.ReadKey(true);
}

// ─────────────────────────────────────────────────────────────
// Error record with full reproduction data
// ─────────────────────────────────────────────────────────────

record GenerationFailure(
    string DisplayName,
    PoemType Type,
    int AttemptNumber,
    int? GlobalSeed,
    int? PoemSeed,
    string ExceptionType,
    string Message,
    string StackTrace
);
