using QuickPulse.Explains;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Reference.B_MakeItFlow.When;


[DocFile]
public class WhenTests
{
    [Fact]
    [DocContent(
@"
**`Pulse.When(...)`** Executes a subflow conditionally.

A flow that does not take an input like `var someMessage = Pulse.Trace(""Some Message"")` can be defined as a sub flow,
and executed by simple including it in the Linq chain: `from _ in someMessage`.

If we want to flow, based on a predicate, we could do: `from _ in predicate ? someMessage : Pulse.NoOp()`.

Which is fine but with `Pulse.When(...)` we can do better.

**Example:**
```csharp
var dotDotDot = Pulse.Trace(""..."");
var flow =
    from anInt in Pulse.Start<int>()
    from _ in Pulse.When(anInt == 42, dotDotDot) // <=
    select anInt;
var collector = new TheCollector<string>();
Signal.From(flow).SetArtery(collector)
    .Pulse(6)
    .Pulse(42);
Assert.Equal([""...""], collector.TheExhibit);
```
")]
    public void Pulse_when()
    {
        var dotDotDot = Pulse.Trace("...");
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.When(anInt == 42, dotDotDot)
            select anInt;
        var collector = TheCollector.Exhibits<string>();
        Signal.From(flow).SetArtery(collector)
            .Pulse(6)
            .Pulse(42);
        Assert.Equal(["..."], collector.TheExhibit);
    }
}