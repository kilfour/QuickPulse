using QuickPulse.Explains;
using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Tests.Docs.F_ArteriesIncluded;


[DocFile]
[DocContent("QuickPulse comes with a couple of built-in arteries:")]
public class ArteriesIncluded
{
    [Fact]
    [DocHeader("The Shunt, a.k.a. `/dev/null`")]
    [DocContent(
@"The **Shunt** is the default artery installed in every new signal.  
It implements the Null Object pattern: an inert artery that silently absorbs all data.
Any call to `Absorb()` on a shunt simply vanishes, no storage, no side effects, no errors.
This ensures that flows without an explicitly attached artery still execute safely.")]
    public void TheShunt_Is_The_Default()
        => Assert.NotNull(Signal.From<string>(a => Pulse.Trace(a)).GetArtery<Shunt>());

    [Fact]
    [DocHeader("The Collector")]
    [DocContent(
@"The **Collector** is a simple artery that **gathers** every absorbed value into an internal collection.
It is primarily used in tests and diagnostics to verify what data a signal emits.
Each call to `Absorb()` appends the incoming objects to the exhibit list, preserving order.
Think of it as a **curator** for your flows, nothing escapes notice, everything is archived for later inspection.

Example:")]
    [DocExample(typeof(ArteriesIncluded), nameof(TheCollector_Usage_Example))]
    public void TheCollector_Usage()
    {
        var collector = TheCollector_Usage_Example();
        Assert.Equal("hello", collector.TheExhibit[0]);
        Assert.Equal("collector", collector.TheExhibit[1]);
    }

    [CodeSnippet]
    [CodeRemove("return collector;")]
    private static Collector<string> TheCollector_Usage_Example()
    {
        var collector = TheCollector.Exhibits<string>();
        Signal.From<string>(a => Pulse.Trace(a))
            .SetArtery(collector)
            .Pulse("hello")
            .Pulse("collector");
        // collector.TheExhibit now equals ["hello", "collector"]
        return collector;
    }

    [Fact]
    [DocHeader("The Latch")]
    [DocContent(
@"The **Latch** is a tiny, type-safe last-value latch. It simply remembers the most recent value absorbed and exposes it via `Q`.  
This is ideal for tests and probes where you only care about what came out last.

Example:")]
    [DocExample(typeof(ArteriesIncluded), nameof(TheLatch_Usage_Example))]
    public void TheLatch_Usage()
    {
        var latch = TheLatch_Usage_Example();
        Assert.Equal("latch", latch.Q);
    }

    [CodeSnippet]
    [CodeRemove("return latch;")]
    private static Latch<string> TheLatch_Usage_Example()
    {
        var latch = TheLatch.Holds<string>();
        Signal.From<string>(a => Pulse.Trace(a))
            .SetArtery(latch)
            .Pulse("hello")
            .Pulse("latch");
        // latch.Q now equals "latch"
        return latch;
    }

    [Fact]
    [DocHeader("The Ledger")]
    [DocContent(
@"The **Ledger**` is a **persistent artery**, it records every absorbed value into a file.
Where `TheCollector` keeps its exhibits in memory, `TheLedger` writes them down for posterity.
It is ideal for tracing long-running flows or auditing emitted data across multiple runs.
Think of it as your **flow accountant**, keeping a faithful record of every transaction.  

Example:
")]
    [DocExample(typeof(ArteriesIncluded), nameof(TheLedger_Usage_Example))]
    public void TheLedger_Usage()
    {
        var ledger = TheLedger_Usage_Example();
        var filePath = ledger.FilePath;
        var lines = File.ReadAllLines(filePath);
        Assert.Equal("hello", lines[0]);
        Assert.Equal("filesystem", lines[1]);
        File.Delete(filePath);
    }

    [CodeSnippet]
    [CodeRemove("return ledger;")]
    private static Ledger TheLedger_Usage_Example()
    {
        var ledger = TheLedger.Records();
        Signal.From<string>(a => Pulse.Trace(a))
            .SetArtery(ledger)
            .Pulse("hello")
            .Pulse("filesystem");
        // File.ReadAllLines(...) now equals ["hello", "filesystem"]
        return ledger;
    }

    [Fact]
    [DocContent(
@"When a filename is not explicitly provided, a unique file is automatically created in a .quickpulse directory
located at the solution root (i.e., the nearest parent directory containing a .sln file).  

The filename follows this pattern:
```bash
/solution/.quickpulse/quick-pulse-{unique-suffix}.log
```
This ensures that each run generates a distinct, traceable log file without overwriting previous logs.
")]
    public void Default_constructor_uses_quick_dash_pulse_dot_log()
    {
        // var fake = new FakeFilingCabinet();
        // _ = new Ledger(cabinet: fake);
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
        Assert.EndsWith($"{Path.DirectorySeparatorChar}myfilename.log", Constructor_uses_custom_filename_example());
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static string Constructor_uses_custom_filename_example()
    {
        return TheLedger.Records("myfilename.log").FilePath;
    }

    [DocContent(
@"Note that the `Ledger` will throw an exception if no `.sln` file can be found.")]
    [Fact(Skip = "use temp dir")]
    public void Throws_when_solution_root_is_null()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => new Ledger());
        Assert.Equal("Cannot find solution root.", ex.Message);
    }

    [Fact]
    [DocContent(
@"The `TheLedger.Rewrites()` factory method does exactly what it says: it clears the file before logging.
This is an idiomatic way to log repeatedly to a file that should start out empty:")]
    public void ClearFile_writes_empty_string_to_file()
    {
        // Setup a file with content
        var filePath =
            Signal.From<string>(a => Pulse.Trace(a))
                .SetArtery(TheLedger.Records("myfilename.log"))
                .Pulse(["hello", "filesystem"])
                .GetArtery<Ledger>()
                .FilePath;
        var lines = File.ReadAllLines(filePath);
        Assert.Equal("hello", lines[0]);
        Assert.Equal("filesystem", lines[1]);
        // Clear it
        Signal.From<string>(a => Pulse.Trace(a))
            .SetArtery(TheLedger.Rewrites("myfilename.log"))
            .GetArtery<Ledger>();
        Assert.Equal(string.Empty, File.ReadAllText(filePath));
        File.Delete(filePath);
    }

    [Fact]
    [DocHeader("The String Catcher")]
    [DocContent(
@"This catcher quietly captures everything that flows through it, and returns it as a single string.  
It is especially useful in testing and example scenarios where the full trace output is needed as a value.

Use the static helper `TheString.Catcher()` to create a new catcher.")]
    public void TheStringCatcher()
    {
        var artery = TheString.Catcher();
        Assert.IsType<Holden>(artery);
    }

    [Fact]
    [DocContent("You can get a hold of the string through the `.Whispers()` method.")]
    [DocExample(typeof(ArteriesIncluded), nameof(TheStringCatcher_Whispers_Example))]
    public void TheStringCatcher_Whispers()
        => Assert.Equal("x = 42", TheStringCatcher_Whispers_Example());

    [CodeSnippet]
    [CodeRemove("return result;")]
    private static string TheStringCatcher_Whispers_Example()
    {
        var holden = TheString.Catcher();
        Signal.From(
            from x in Pulse.Start<int>()
            from _ in Pulse.Trace("x = ")
            from __ in Pulse.Trace(42)
            select x)
        .SetArtery(holden)
        .Pulse(42);
        var result = holden.Whispers(); // <=
        // result now equals "x = 42"
        return result;
    }

    [Fact]
    [DocContent("You can also reset/clear the *caught* values using the `.Forgets()` method.")]
    public void TheStringCatcher_Forgets()
    {
        var holden = TheString.Catcher();
        var signal =
            Signal.From(
                    from x in Pulse.Start<int>()
                    from _ in Pulse.Trace("x = ")
                    from __ in Pulse.Trace(42)
                    select x)
                .SetArtery(holden)
                .Pulse(1);
        holden.Forgets();
        signal.Pulse(42);
        Assert.Equal("x = 42", holden.Whispers());
    }
}


