using QuickPulse.Bolts;
using QuickPulse.Tests._Tools;

namespace QuickPulse.Tests.BuildFlowTests;


[Doc(Order = Chapters.BuildFlow, Caption = "Building a Flow", Content =
@"In order to explain how QuickPulse works, not in the least to myself, let's walk through 
building up a flow.
")]
public class PulseBuildFlowTests
{
    [Doc(Order = Chapters.BuildFlow + "-1", Content =
@"The minimal definition of a flow :
```csharp
from start in Pulse.Start<string>()
select start;
```
The type generic in `Pulse.Start` defines the **input type** to the flow.  
**Note:** It is required to select the result of `Pulse.Start` at the end of the LINQ chain for the flow to be considered well-formed.
")]
    [Fact]
    public void Minimal_definition_start()
    {
        var flow =
            from start in Pulse.Start<string>()
            select start;
        Assert.IsType<Flow<string>>(flow);
    }

    [Doc(Order = Chapters.BuildFlow + "-2", Content =
@"Let's start doing something with that input value.
Let's send it out into the world.
Example:
```csharp
from str in Pulse.Start<string>()
from trace in Pulse.Trace(str)
select str;
```
")]
    [Fact]
    public void Adding_a_trace()
    {
        var flow =
            from str in Pulse.Start<string>()
            from trace in Pulse.Trace(str)
            select str;
        Assert.IsType<Flow<string>>(flow);
    }

    [Doc(Order = Chapters.BuildFlow + "-4", Content =
@"Above example in itself


")]
    [Fact]
    public void Adding_an_Artery()
    {
        var flow =
            from start in Pulse.Start<string>()
                //let caption = doc.Caption
            select start;
        Assert.IsType<Flow<string>>(flow);
    }

    [Doc(Order = Chapters.BuildFlow + "-3", Content =
@"Creating a `Signal` and emmiting a `Pulse`.
Example: 
```csharp
Signal.From(flow).Pulse(""a string value"");
```
This sends the ""a string value"" parameter into the flow.
")]
    [Fact]
    public void Filtering()
    {
        var flow =
            from start in Pulse.Start<string>()
            from _ in Pulse.Trace(() => { })
            select start;
        Assert.IsType<Flow<string>>(flow);
    }

}