using QuickPulse.Explains;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace QuickPulse.Tests.Docs.BuildingAFlow;


[Doc(Order = Chapters.BuildFlow, Caption = "Building a Flow", Content =
@"To explain how QuickPulse works (not least to myself), let's build up a flow step by step.")]
public class PulseBuildFlowTests
{
    [Doc(Order = Chapters.BuildFlow + "-1", Caption = "The Minimal Flow", Content =
@"
```csharp
from anInt in Pulse.Start<int>()
select anInt;
```

The type generic in `Pulse.Start<T>` defines the **input type** to the flow.
**Note:** It is required to select the result of `Pulse.Start(...)` at the end of the LINQ chain for the flow to be considered well-formed.")]
    [Fact]
    public void Minimal_definition_start()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            select anInt;
        Assert.IsType<Flow<int>>(flow);
    }

    [Doc(Order = Chapters.BuildFlow + "-2", Caption = "Doing Something with the Input", Content =
@"Let's trace the values as they pass through:

```csharp
from anInt in Pulse.Start<int>()
from trace in Pulse.Trace(anInt)
select anInt;
```
")]
    [Fact]
    public void Adding_a_trace()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            from trace in Pulse.Trace(anInt)
            select anInt;
        Assert.IsType<Flow<int>>(flow);
    }

    [Doc(Order = Chapters.BuildFlow + "-3", Caption = "Executing a Flow", Content =
@"To execute a flow, we need a `Signal<T>`, which is created via:

```csharp
Signal.From<T>(Flow<T> flow)
```

Example:

```csharp
var flow =
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;

var signal = Signal.From(flow);
```
")]
    [Fact]
    public void Adding_an_Signal()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            from trace in Pulse.Trace(anInt)
            select anInt;
        var signal = Signal.From(flow);
        Assert.IsType<Signal<int>>(signal);
    }

    [Doc(Order = Chapters.BuildFlow + "-4", Caption = "Sending Values Through the Flow", Content =
@"Once you have a signal, you can push values into the flow by calling:

```csharp
Signal.Pulse(params T[] input)
```

Example:

```csharp
var flow =
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;

var signal = Signal.From(flow);
signal.Pulse(42);
```

This sends the value `42` into the flow.
")]
    [Fact]
    public void Adding_a_pulse()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            from trace in Pulse.Trace(anInt)
            select anInt;
        var signal = Signal.From(flow);
        signal.Pulse(42);
        Assert.IsType<Flow<int>>(flow);
    }

    [Doc(Order = Chapters.BuildFlow + "-5", Caption = "Capturing the Trace", Content =
@"To observe what flows through, we can add an `IArtery` by using `SetArtery` directly on the signal.

```csharp
[Fact]
public void Adding_an_artery()
{
    var flow =
        from anInt in Pulse.Start<int>()
        from trace in Pulse.Trace(anInt)
        select anInt;

    var collector = new TheCollector<int>();

    Signal.From(flow)
        .SetArtery(collector)
        .Pulse(42, 43, 44);

    Assert.Equal(3, collector.TheExhibit.Count);
    Assert.Equal(42, collector.TheExhibit[0]);
    Assert.Equal(43, collector.TheExhibit[1]);
    Assert.Equal(44, collector.TheExhibit[2]);
}
```

")]
    [Fact]
    public void Adding_an_artery()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            from trace in Pulse.Trace(anInt)
            select anInt;
        var collector = new TheCollector<int>();
        Signal.From(flow)
            .SetArtery(collector)
            .Pulse(42, 43, 44);
        Assert.Equal(3, collector.TheExhibit.Count);
        Assert.Equal(42, collector.TheExhibit[0]);
        Assert.Equal(43, collector.TheExhibit[1]);
        Assert.Equal(44, collector.TheExhibit[2]);
    }
}