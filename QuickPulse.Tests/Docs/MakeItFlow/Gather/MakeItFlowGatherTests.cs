using QuickPulse.Explains;
using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Tests.Docs.MakeItFlow.Gather;


public class MakeItFlowGatherTests
{
    [Doc(Order = Chapters.MakeItFlow, Caption = "Gather", Content =
@"**`Pulse.Gather(...)`** Binds a mutable box into flow memory (first write wins).

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1) // <=
select anInt;
```")]
    [Fact]
    public void Pulse_gather()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from box in Pulse.Gather(1)
            from _ in Pulse.Trace(anInt + box.Value)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(41);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.MakeItFlow + "-1", Caption = "", Content =
@"**`Pulse.Gather<T>()`** used without an argument, serves as a 'getter' of a previously gathered value.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1)
from val in Pulse.Gather<int>() // <=
from _ in Pulse.Trace(anInt + val.Value)
select anInt;
```")]
    [Fact]
    public void Pulse_gather_empty_param()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from box in Pulse.Gather(1)
            from val in Pulse.Gather<int>()
            from _ in Pulse.Trace(anInt + val.Value)
            select anInt;
        Signal.From(flow).SetArtery(collector).Pulse(41);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.MakeItFlow + "-2", Caption = "", Content =
 @"**`Pulse.Gather<T>()`** throws if no value of the requested type is available.")]
    [Fact]
    public void Pulse_gather_empty_param_no_value_throws()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            from val in Pulse.Gather<int>()
            from _ in Pulse.Trace(anInt + val.Value)
            select anInt;
        var ex = Assert.Throws<ComputerSaysNo>(() => Signal.From(flow).Pulse(42));
        Assert.Equal("No value of type Int32 found.", ex.Message);
    }
}