using QuickPulse.Explains.Deprecated;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Docs.MakeItFlow.Trace;


public class MakeItFlowTraceTests
{
    [Doc(Order = Chapters.MakeItFlow, Caption = "Trace", Content =
@"
**`Pulse.Trace(...)`** emits trace data unconditionally to the current artery.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.Trace(anInt) // <=
select anInt;
```
")]
    [Fact]
    public void Pulse_trace()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Trace(anInt)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.MakeItFlow + "-1", Caption = "", Content =
@"
**`Pulse.TraceIf(...)`** emits trace data conditionally, based on a boolean flag.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.TraceIf(anInt != 42, () => anInt) // <=
select anInt;
```
")]
    [Fact]
    public void Pulse_trace_if()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.TraceIf(anInt != 42, () => anInt)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        signal.Pulse(7);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(7, collector.TheExhibit[0]);
    }
}