using QuickPulse.Explains;
using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Tests.Docs.F_ArteriesIncluded;

[DocFile]
[DocContent("QuickPulse comes with a couple of built-in arteries:")]
public class ArteriesIncluded
{
    [Fact]
    [DocHeader("The NullArtery, a.k.a. `/dev/null`")]
    [DocContent(
@"The **NullArtery** is the default artery installed in every new signal.  
It implements the Null Object pattern: an inert artery that silently absorbs all data.
Any call to `Absorb()` on a shunt simply vanishes, no storage, no side effects, no errors.
This ensures that flows without an explicitly attached artery still execute safely.")]
    public void TheNullArtery_Is_The_Default()
        => Assert.NotNull(Signal.From<string>(a => Pulse.Trace(a)).GetArtery<NullArtery>());

    [Fact]
    [DocHeader("The Collector")]
    [DocContent(
@"The **Collector** is a simple artery that **gathers** every absorbed value into an internal collection.
It is primarily used in tests and diagnostics to verify what data a signal emits.
Each call to `Absorb()` appends the incoming objects to the exhibit list, preserving order.
Think of it as a **curator** for your flows, nothing escapes notice, everything is archived for later inspection.

Example:")]
    [DocExample(typeof(ArteriesIncluded), nameof(Collector_Usage_Example))]
    public void Collector_Usage()
    {
        var collector = Collector_Usage_Example();
        Assert.Equal("hello", collector.Values[0]);
        Assert.Equal("collector", collector.Values[1]);
    }

    [CodeSnippet]
    [CodeRemove("return collector;")]
    private static Collector<string> Collector_Usage_Example()
    {
        var collector = Collect.ValuesOf<string>();
        Signal.From<string>(a => Pulse.Trace(a))
            .SetArtery(collector)
            .Pulse("hello")
            .Pulse("collector");
        // collector.Values now equals ["hello", "collector"]
        return collector;
    }

    [Fact]
    [DocHeader("The FileLogArtery")]
    [DocContent(
@"The `**FileLogArtery**` is a **persistent artery**, it records every absorbed value into a file.
Where the `Collector` keeps its exhibits in memory, The `FileLogArtery` writes them down for posterity.
It is ideal for tracing long-running flows or auditing emitted data across multiple runs.
Think of it as your **flow accountant**, keeping a faithful record of every transaction.  

Example:
")]
    [DocExample(typeof(ArteriesIncluded), nameof(TheFileLog_Usage_Example))]
    public void TheFileLog_Usage()
    {
        var fileLog = TheFileLog_Usage_Example();
        var filePath = fileLog.FilePath;
        var lines = File.ReadAllLines(filePath);
        Assert.Equal("hello", lines[0]);
        Assert.Equal("filesystem", lines[1]);
        File.Delete(filePath);
    }

    [CodeSnippet]
    [CodeRemove("return fileLog;")]
    private static FileLogArtery TheFileLog_Usage_Example()
    {
        var fileLog = FileLog.Append();
        Signal.From<string>(a => Pulse.Trace(a))
            .SetArtery(fileLog)
            .Pulse("hello")
            .Pulse("filesystem");
        // File.ReadAllLines(...) now equals ["hello", "filesystem"]
        return fileLog;
    }

    [Fact]
    [DocContent(
@"When a filename is not explicitly provided, a unique file is automatically created in a .quickpulse directory
located at the nearest directory containing a .sln file (the solution root).  

The filename follows this pattern:
```bash
/solution/.quickpulse/quick-pulse-{unique-suffix}.log
```
This ensures that each run generates a distinct, traceable log file without overwriting previous logs.
")]
    public void Default_constructor_uses_quick_dash_pulse_dot_log()
    {
        // var fake = new FakeFilingCabinet();
        // _ = new FileLog(cabinet: fake);
        // Assert.StartsWith("/solution/.quickpulse/quick-pulse-", fake.LastCombinedPath);
        // Assert.EndsWith(".log", fake.LastCombinedPath);
    }

    [Fact]
    [DocContent(
@"You can, of course, pass in a custom filename.  
In that case, a `myfilename.log` file is created, still in the nearest parent directory that contains a `.sln` file.  

Example:")]
    [DocExample(typeof(ArteriesIncluded), nameof(Constructor_uses_custom_filename_example))]
    public void Constructor_uses_custom_filename()
    {
        Assert.EndsWith($"{Path.DirectorySeparatorChar}myfilename.log", Constructor_uses_custom_filename_example().FilePath);
    }

    [CodeSnippet]
    [CodeRemove("return ")]
    private static FileLogArtery Constructor_uses_custom_filename_example()
    {
        return FileLog.Append("myfilename.log");
    }

    [DocContent(
@"Note that the `FileLogArtery` will throw an exception if no `.sln` file can be found.")]
    [Fact(Skip = "use temp dir")]
    public void Throws_when_solution_root_is_null()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => new FileLogArtery());
        Assert.Equal("Cannot find solution root.", ex.Message);
    }

    [Fact]
    [DocContent(
@"The `FileLogArtery.Writes()` clears the file before logging.
This is an idiomatic way to log repeatedly to a file that should start out empty:")]
    public void ClearFile_writes_empty_string_to_file()
    {
        // Setup a file with content
        var filePath =
            Signal.From<string>(a => Pulse.Trace(a))
                .SetArtery(FileLog.Append("myfilename.log"))
                .Pulse(["hello", "filesystem"])
                .GetArtery<FileLogArtery>()
                .FilePath;
        var lines = File.ReadAllLines(filePath);
        Assert.Equal("hello", lines[0]);
        Assert.Equal("filesystem", lines[1]);
        // Clear it
        Signal.From<string>(a => Pulse.Trace(a))
            .SetArtery(FileLog.Write("myfilename.log"))
            .GetArtery<FileLogArtery>();
        Assert.Equal(string.Empty, File.ReadAllText(filePath));
        File.Delete(filePath);
    }

    [Fact]
    [DocHeader("The String Sink")]
    [DocContent(
@"This artery quietly captures everything that flows through it, and returns it as a single string.  
It is especially useful in testing and example scenarios where the full trace output is needed as a value.

Use the static helper `Text.Capture()` to create a new catcher.")]
    public void TheStringSink()
    {
        var artery = Text.Capture();
        Assert.IsType<StringSink>(artery);
    }

    [Fact]
    [DocContent("You can get a hold of the string through the `.Content()` method.")]
    [DocExample(typeof(ArteriesIncluded), nameof(TheStringSink_Content_Example))]
    public void TheStringSink_Content()
        => Assert.Equal("x = 42", TheStringSink_Content_Example());

    [CodeSnippet]
    [CodeRemove("return result;")]
    private static string TheStringSink_Content_Example()
    {
        var stringSink = Text.Capture();
        Signal.From(
            from x in Pulse.Start<int>()
            from _ in Pulse.Trace("x = ")
            from __ in Pulse.Trace(42)
            select x)
        .SetArtery(stringSink)
        .Pulse(42);
        var result = stringSink.Content(); // <=
        // result now equals "x = 42"
        return result;
    }

    [Fact]
    [DocContent("You can also reset/clear the *caught* values using the `.Clear()` method.")]
    public void TheStringSink_Clear()
    {
        var stringSink = Text.Capture();
        var signal =
            Signal.From(
                    from x in Pulse.Start<int>()
                    from _ in Pulse.Trace("x = ")
                    from __ in Pulse.Trace(42)
                    select x)
                .SetArtery(stringSink)
                .Pulse(1);
        stringSink.Clear();
        signal.Pulse(42);
        Assert.Equal("x = 42", stringSink.Content());
    }
}


