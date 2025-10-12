using QuickPulse.Explains;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Reference.B_MakeItFlow.NoOp;

[DocFile]
public class NoOpTests
{
    [Fact]
    [DocContent(
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
        var collector = TheCollector.Exhibits<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        Assert.Empty(collector.TheExhibit);
    }
}