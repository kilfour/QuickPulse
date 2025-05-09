using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Tests._Tools;

namespace QuickPulse.Tests.BuildFlowTests;


[Doc(Order = Chapters.HowToPulse, Caption = "How To Pulse", Content =
@"")]
public class PulseHowToPulseTests
{
    [Doc(Order = Chapters.HowToPulse + "-1", Caption = "Start", Content =
@"
**`Pulse.Start<T>()`** is explained in a previous chapter, but for completeness sake, here's a quick recap.

Every flow definition needs to start with a call to `Pulse.Start<T>()`.
This strongly types the values that the flow can receive.
In adition the result of the call needs to be used in the select part of the LINQ expression.
This strongly types the flow itself.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
select anInt;
```
")]
    [Fact]
    public void Pulse_start()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            select anInt;

        Assert.IsType<Flow<int>>(flow);
    }
}