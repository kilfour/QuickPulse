using QuickPulse.Explains.Deprecated;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Docs.MakeItFlow.NoOp;


public class MakeItFlowNoOpTests
{
    [Fact]
    [Doc(Order = Chapters.MakeItFlow, Caption = "NoOp", Content =
@"
**`Pulse.NoOp(...)`** A do-nothing operation (useful for conditional branches). 

**Example:**
```csharp
from anInt in Pulse.Start<int>()
    from _ in Pulse
        .NoOp(/* --- Also useful for Comments --- */)
    select anInt;
```
")]
    public void Pulse_no_op()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse
                .NoOp(/* --- Also useful for Comments --- */)
            select anInt;
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        Assert.Empty(collector.TheExhibit);
    }
}