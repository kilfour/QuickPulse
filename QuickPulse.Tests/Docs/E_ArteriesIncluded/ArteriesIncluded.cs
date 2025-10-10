using QuickPulse.Explains;
using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Tests.Docs.E_ArteriesIncluded;


//[DocFile]
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
        => Assert.NotNull(Signal.Tracing<string>().GetArtery<Shunt>());

    [Fact]
    [DocHeader("The Collector")]
    [DocContent(
@"The **Collector** is a simple artery that **gathers** every absorbed value into an internal collection.
It is primarily used in tests and diagnostics to verify what data a signal emits.
Each call to `Absorb()` appends the incoming objects to the exhibit list, preserving order.
Think of it as a **curator** for your flows, nothing escapes notice, everything is archived for later inspection.

Example:")]
    [DocExample(typeof(ArteriesIncluded), nameof(TheCollector_Example))]
    [CodeSnippet]
    public void TheCollector_Example()
    {
        var collector = TheCollector.Exhibits<string>();
        Signal.Tracing<string>()
            .SetArtery(collector)
            .Pulse(["hello", "collector"]);
        Assert.Equal("hello", collector.TheExhibit[0]);
        Assert.Equal("collector", collector.TheExhibit[1]);
    }

    [Fact]
    [DocHeader("The Latch")]
    [DocContent(
@"The **Latch** is a tiny, type-safe last-value latch. It simply remembers the most recent value absorbed and exposes it via `Q`.  
This is ideal for tests and probes where you only care about what came out last.

Example:")]
    [DocExample(typeof(ArteriesIncluded), nameof(TheLatch_Example))]
    [CodeSnippet]
    public void TheLatch_Example()
    {
        var latch = TheLatch.Holds<string>();
        Signal.Tracing<string>()
            .SetArtery(latch)
            .Pulse(["hello", "latch"]);
        Assert.Equal("latch", latch.Q);
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
    public void TheLedger_Example()
    {
        //     var ledger = TheLedger.Records();
        //     Signal.Tracing<string>()
        //         .SetArtery(ledger)
        //         .Pulse(["hello", "ledger"]);
        //     var expectedPath = fake.GetFullPath("/solution/.quickpulse/quick-pulse-SUFFIX.log");
        //     Assert.Collection(fake.Appends,
        //        item => Assert.Equal((expectedPath, "hello" + Environment.NewLine), fake.Appends[0]),
        //        item => Assert.Equal((expectedPath, "collector" + Environment.NewLine), fake.Appends[1])
        //    );
    }



    // [CodeSnippet]
    private void TheLedger_Example_Usage()
    {
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

    [DocContent(
@"You can, of course, pass in a custom filename.
Example:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile(""myfilename.log""))
    .Pulse(""hello"", ""collector"");
```
In that case, a `myfilename.log` file is created, still in the nearest parent directory that contains a `.sln` file.
")]
    [Fact]
    public void Constructor_uses_custom_filename()
    {
        // var artery = new Ledger("custom.txt");
        // var expected = fake.GetFullPath("/solution/custom.txt");
        // artery.ClearFile(); // Triggers a write
        // Assert.Contains((expected, ""), fake.Writes);
    }

    [DocContent(
@"Note that the `WriteDataToFile` constructor will throw an exception if no `.sln` file can be found.")]
    [Fact(Skip = "use temp dir")]
    public void Throws_when_solution_root_is_null()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => new Ledger());
        Assert.Equal("Cannot find solution root.", ex.Message);
    }



    [DocContent(
@"`WriteDataToFile` appends all entries to the file; each pulse adds new lines to the end.
")]
    [Fact]
    public void Flow_appends_all_lines()
    {
        // var artery = new Ledger("data.txt");
        // artery.Absorb("one", 2, "three");
        // var expectedPath = fake.GetFullPath("/solution/data.txt");
        // Assert.Collection(fake.Appends,
        //     item => Assert.Equal((expectedPath, "one" + Environment.NewLine), item),
        //     item => Assert.Equal((expectedPath, "2" + Environment.NewLine), item),
        //     item => Assert.Equal((expectedPath, "three" + Environment.NewLine), item)
        // );
    }

    [DocContent(
@"The `ClearFile` method does exactly what it says: it clears the file before logging.
This is an idiomatic way to log repeatedly to a file that should start out empty:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile().ClearFile())
    .Pulse(""hello"", ""collector"");
```
")]
    [Fact]
    public void ClearFile_writes_empty_string_to_file()
    {
        // var fake = new FakeFilingCabinet();
        // var artery = new Ledger("clear-me.txt");
        // artery.ClearFile();
        // var expected = fake.GetFullPath("/solution/clear-me.txt");
        // Assert.Contains((expected, ""), fake.Writes);
    }



    [Fact]
    [DocContent(
@"- `WriteData.ToNewFile(...)` is shorthand for `new WriteDataToFile(...).ClearFile()`")]
    public void ToNewFile()
    {
        // -------------------------------------------------------
        // Untested as it creates a log file on execution
        //
        // var artery = WriteData.ToNewFile();
        // Assert.IsType<WriteDataToFile>(artery);
    }

    [Fact]
    [DocHeader("TheStringCatcher")]
    [DocContent(
@"This catcher quietly captures everything that flows through it, and returns it as a single string.  
It is especially useful in testing and example scenarios where the full trace output is needed as a value.

Use the static helper `TheString.Catcher()` to create a new catcher:
```csharp
var holden = TheString.Catcher();
```")]
    public void TheStringCatcher()
    {
        var artery = TheString.Catcher();
        Assert.IsType<Holden>(artery);
    }

    [Fact]
    [DocContent(
@"You can get a hold of the string through the `.Whispers()` method.
```csharp
var holden = TheString.Catcher();
Signal.From(
        from x in Pulse.Start<int>()
        from _ in Pulse.Trace($""x = {x}"")
        select x)
    .SetArtery(holden)
    .Pulse(42);
Assert.Equal(""x = 42"", holden.Whispers());
```
Great for assertions or string comparisons in tests.")]
    public void TheStringCatcher_Whispers()
    {
        var holden = TheString.Catcher();
        Signal.From(
                from x in Pulse.Start<int>()
                from _ in Pulse.Trace($"x = {x}")
                select x)
            .SetArtery(holden)
            .Pulse(42);
        Assert.Equal("x = 42", holden.Whispers());
    }
}


