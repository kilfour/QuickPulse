using QuickExplainIt;
using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Tests.Docs.BuildFlowTests;


[Doc(Order = Chapters.ArteriesIncluded, Caption = "Arteries Included", Content =
@"QuickPulse comes with *only* two build in arteries:")]
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
            .Pulse("hello", "collector");
        Assert.Equal("hello", collector.TheExhibit[0]);
        Assert.Equal("collector", collector.TheExhibit[1]);
    }

    [Doc(Order = Chapters.ArteriesIncluded + "-2", Caption = "WriteDataToFile", Content =
@"This artery is included because writing trace output to a file is one of the most common use cases.
Example:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile())
    .Pulse(""hello"", ""collector"");
```
By default, this creates a `quick-pulse.log` file in the nearest parent directory that contains a `.sln` file, typically the solution root.
The file will contain:
```
hello
collector
```
")]
    [Fact]
    public void WriteDataToFile_Example()
    {
        var fake = new FakeFilingCabinet();
        var collector = new WriteDataToFile(cabinet: fake);
        Signal.Tracing<string>()
            .SetArtery(collector)
            .Pulse("hello", "collector");
        var expectedPath = fake.GetFullPath("/solution/quick-pulse.log");
        Assert.Collection(fake.Appends,
           item => Assert.Equal((expectedPath, "hello" + Environment.NewLine), item),
           item => Assert.Equal((expectedPath, "collector" + Environment.NewLine), item)
       );
    }

    [Fact]
    public void Default_constructor_uses_log_txt()
    {
        var fake = new FakeFilingCabinet();
        _ = new WriteDataToFile(cabinet: fake); // bad test this one
        Assert.Contains("/solution/quick-pulse.log", fake.GetFullPath("/solution/quick-pulse.log"));
    }

    [Doc(Order = Chapters.ArteriesIncluded + "-3", Caption = "", Content =
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

    [Doc(Order = Chapters.ArteriesIncluded + "-4", Caption = "", Content =
@"Note that the `WriteDataToFile` constructor will throw an exception if no `.sln` file can be found.")]
    [Fact]
    public void Throws_when_solution_root_is_null()
    {
        var fake = new FakeFilingCabinet { SolutionRoot = null };
        var ex = Assert.Throws<ComputerSaysNo>(() => new WriteDataToFile(cabinet: fake));
        Assert.Equal("Cannot find solution root.", ex.Message);
    }

    [Doc(Order = Chapters.ArteriesIncluded + "-5", Caption = "", Content =
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


    [Doc(Order = Chapters.ArteriesIncluded + "-6", Caption = "", Content =
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

    [Doc(Order = Chapters.ArteriesIncluded + "-7", Caption = "", Content =
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

    [Doc(Order = Chapters.ArteriesIncluded + "-8-1", Caption = "Sugaring", Content =
@"I usually prefer bitter, but adding a bit of sweet sometimes doesn't hurt.
-  `WriteData.ToFile(...)` is the same as `new WriteDataToFile()`.
-  `WriteData.ToNewFile(...)` is the same as `new WriteDataToFile().ClearFile()`.
")]
    [Fact]
    public void Sugaring()
    {
        var artery = WriteData.ToFile();
        Assert.IsType<WriteDataToFile>(artery);
    }
}


