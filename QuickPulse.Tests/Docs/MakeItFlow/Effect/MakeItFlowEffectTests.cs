using QuickPulse.Explains.Deprecated;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Docs.MakeItFlow.Effect;


public class MakeItFlowEffectTests
{
    [Doc(Order = Chapters.MakeItFlow, Caption = "Effect", Content =
@"
**`Pulse.Effect(...)`** Executes a side-effect without yielding a value.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1)
from eff in Pulse.Effect(() => box.Value++) // <=
select anInt;
```
**Warning:** `Effect` performs side-effects.
It is eager, observable, and runs even if you ignore the result.
Use when you mean it.
")]
    [Fact]
    public void Pulse_effect()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from box in Pulse.Gather(1)
            from eff in Pulse.Effect(() => box.Value++)
            from _ in Pulse.Trace(anInt + box.Value)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(40);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.MakeItFlow + ".7", Caption = "", Content =
@"
**`Pulse.EffectIf(...)`** Same as above, but conditional. 

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from seen42 in Pulse.Gather(false)
from eff in Pulse.EffectIf(anInt == 42, () => seen42.Value = true) // <=
select anInt;
```
")]
    [Fact]
    public void Pulse_effect_if()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from seen42 in Pulse.Gather(false)
            from eff in Pulse.EffectIf(anInt == 42, () => seen42.Value = true)
            from _ in Pulse.TraceIf<bool>(a => a, () => anInt)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(40);
        signal.Pulse(42);
        signal.Pulse(7);
        Assert.Equal(2, collector.TheExhibit.Count);
        Assert.Equal(42, collector.TheExhibit[0]);
        Assert.Equal(7, collector.TheExhibit[1]);
    }
}