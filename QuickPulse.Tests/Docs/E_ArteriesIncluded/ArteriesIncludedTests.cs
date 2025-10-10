using QuickPulse.Explains.Deprecated;
using QuickPulse.Arteries;
using QuickPulse.Instruments;
using QuickPulse.Tests.Docs.BuildFlowTests;

namespace QuickPulse.Tests.Docs.ArteriesIncluded;


[Doc(Order = Chapters.ArteriesIncluded, Caption = "Arteries Included", Content =
@"QuickPulse comes with three built-in arteries:")]
public class ArteriesIncludedTests
{

    [Doc(Order = Chapters.ArteriesIncluded + "-1", Caption = "TheCollector", Content =
@"This is the artery used throughout the documentation examples, and it's especially useful in testing scenarios.

Example:
```csharp
var collector = new TheCollector<string>();
Signal.Tracing<string>()
    .SetArtery(collector)
    .Pulse(""hello"", ""collector"");
Assert.Equal(""hello"", collector.TheExhibit[0]);
Assert.Equal(""collector"", collector.TheExhibit[1]);
```
")]
    [Fact]
    public void TheCollector_Example()
    {
        var collector = new TheCollector<string>();
        Signal.Tracing<string>()
            .SetArtery(collector)
            .Pulse(["hello", "collector"]);
        Assert.Equal("hello", collector.TheExhibit[0]);
        Assert.Equal("collector", collector.TheExhibit[1]);
    }


    [Fact]
    [Doc(Order = Chapters.ArteriesIncluded + "-2", Caption = "WriteDataToFile", Content =
@"This artery is included because writing trace output to a file is one of the most common use cases.
Example:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile())
    .Pulse(""hello"", ""collector"");
```
The file will contain:
```
hello
collector
```
")]
    public void WriteDataToFile_Example()
    {
        var fake = new FakeFilingCabinet();
        var collector = new WriteDataToFile(cabinet: fake);
        Signal.Tracing<string>()
            .SetArtery(collector)
            .Pulse(["hello", "collector"]);
        var expectedPath = fake.GetFullPath("/solution/.quickpulse/quick-pulse-SUFFIX.log");
        Assert.Collection(fake.Appends,
           item => Assert.Equal((expectedPath, "hello" + Environment.NewLine), item),
           item => Assert.Equal((expectedPath, "collector" + Environment.NewLine), item)
       );
    }

    [Fact]
    [Doc(Order = Chapters.ArteriesIncluded + "-2-1", Caption = "", Content =
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
        var fake = new FakeFilingCabinet();
        _ = new WriteDataToFile(cabinet: fake);
        Assert.StartsWith("/solution/.quickpulse/quick-pulse-", fake.LastCombinedPath);
        Assert.EndsWith(".log", fake.LastCombinedPath);
    }

    [Doc(Order = Chapters.ArteriesIncluded + "-2-2", Caption = "", Content =
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
        var fake = new FakeFilingCabinet();
        var artery = new WriteDataToFile("custom.txt", fake);
        var expected = fake.GetFullPath("/solution/custom.txt");
        artery.ClearFile(); // Triggers a write
        Assert.Contains((expected, ""), fake.Writes);
    }

    [Doc(Order = Chapters.ArteriesIncluded + "-2-3", Caption = "", Content =
@"Note that the `WriteDataToFile` constructor will throw an exception if no `.sln` file can be found.")]
    [Fact]
    public void Throws_when_solution_root_is_null()
    {
        var fake = new FakeFilingCabinet { SolutionRoot = null };
        var ex = Assert.Throws<ComputerSaysNo>(() => new WriteDataToFile(cabinet: fake));
        Assert.Equal("Cannot find solution root.", ex.Message);
    }

    [Doc(Order = Chapters.ArteriesIncluded + "-2-4", Caption = "", Content =
@"To avoid solution root detection altogether, use the following factory method:
```csharp
Signal.Tracing<string>()
    .SetArtery(WriteDataToFile.UsingHardCodedPath(""hard.txt""))
    .Pulse(""hello"", ""collector"");
```
")]
    [Fact]
    public void UsingHardCodedPath_resolves_directly()
    {
        var fake = new FakeFilingCabinet();
        var artery = WriteDataToFile.UsingHardCodedPath("hard.txt", fake);

        var expected = fake.GetFullPath("hard.txt");
        artery.ClearFile();

        Assert.Contains((expected, ""), fake.Writes);
    }


    [Doc(Order = Chapters.ArteriesIncluded + "-2-5", Caption = "", Content =
@"`WriteDataToFile` appends all entries to the file; each pulse adds new lines to the end.
")]
    [Fact]
    public void Flow_appends_all_lines()
    {
        var fake = new FakeFilingCabinet();
        var artery = new WriteDataToFile("data.txt", fake);
        artery.Flow("one", 2, "three");
        var expectedPath = fake.GetFullPath("/solution/data.txt");
        Assert.Collection(fake.Appends,
            item => Assert.Equal((expectedPath, "one" + Environment.NewLine), item),
            item => Assert.Equal((expectedPath, "2" + Environment.NewLine), item),
            item => Assert.Equal((expectedPath, "three" + Environment.NewLine), item)
        );
    }

    [Doc(Order = Chapters.ArteriesIncluded + "-2-6", Caption = "", Content =
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
        var fake = new FakeFilingCabinet();
        var artery = new WriteDataToFile("clear-me.txt", fake);
        artery.ClearFile();
        var expected = fake.GetFullPath("/solution/clear-me.txt");
        Assert.Contains((expected, ""), fake.Writes);
    }

    [Fact]
    [Doc(Order = Chapters.ArteriesIncluded + "-2-7", Caption = "Sugaring", Content =
@"These are simple aliases that make common cases easier to read:
- `WriteData.ToFile(...)` is shorthand for `new WriteDataToFile(...)`")]
    public void ToFile()
    {
        var artery = WriteData.ToFile();
        Assert.IsType<WriteDataToFile>(artery);
    }

    [Fact]
    [Doc(Order = Chapters.ArteriesIncluded + "-2-7-1", Caption = "", Content =
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
    [Doc(Order = Chapters.ArteriesIncluded + "-3", Caption = "TheStringCatcher", Content =
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
    [Doc(Order = Chapters.ArteriesIncluded + "-3-1", Caption = "", Content =
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


