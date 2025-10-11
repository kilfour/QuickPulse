using QuickPulse.Explains;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Reference.B_MakeItFlow.Scoped;

[DocFile]
public class ScopedTests
{
    [DocContent(
@"**`Pulse.Scoped<T>(...)`** temporarily alters gathered state of type T, runs an inner flow,
and reverts the state after.
**Example:**
```csharp
var collector = TheCollector.Exhibits<int>();
var innerFlow =
    from anInt in Pulse.Start<int>()
    from scopedBox in Pulse.Gather<int>()
    from _ in Pulse.Trace(anInt + scopedBox.Value)
    select anInt;
var flow =
    from anInt in Pulse.Start<int>()
    from box in Pulse.Prime(()0)
    from _ in Pulse.Trace(anInt + box.Value)
    from scopeInt in Pulse.Scoped<int>(
        a => a + 1,
        Pulse.ToFlow(innerFlow, anInt))
    from __ in Pulse.Trace(anInt + box.Value)
    select anInt;
var signal = Signal.From(flow).SetArtery(collector);
signal.Pulse(42);
Assert.Equal([42, 43, 42], collector.TheExhibit);
```")]
    [Fact]
    public void Pulse_Scoped()
    {
        var collector = TheCollector.Exhibits<int>();
        var innerFlow =
            from anInt in Pulse.Start<int>()
            from scopedBox in Pulse.Draw<int>()
            from _ in Pulse.Trace(anInt + scopedBox)
            select anInt;
        var flow =
            from anInt in Pulse.Start<int>()
            from box in Pulse.Prime(() => 0)
            from _ in Pulse.Trace(anInt + box)
            from scopeInt in Pulse.Scoped<int>(
                a => a + 1,
                Pulse.ToFlow(innerFlow, anInt))
            from __ in Pulse.Trace(anInt + box)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        Assert.Equal([42, 43, 42], collector.TheExhibit);
    }
}