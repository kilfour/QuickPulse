using QuickPulse.Explains.Deprecated;

namespace QuickPulse.Tests.Docs.MakeItFlow.Start;


public class MakeItFlowStartTests
{
    [Doc(Order = Chapters.MakeItFlow, Caption = "Start", Content =
@"Every flow definition needs to start with a call to `Pulse.Start()`.
This strongly types the values that the flow can receive.
In addition, the result of the call needs to be used in the select part of the LINQ expression.
This strongly types the flow itself.

**Example:**
```csharp
from anInt in Pulse.Start<int>() // <=
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