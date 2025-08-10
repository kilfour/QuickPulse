using QuickPulse.Explains.Deprecated;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace QuickPulse.Tests.Docs.MakeItFlow.ToFlow;


public class MakeItFlowToFlowTests
{
    [Doc(Order = Chapters.MakeItFlow, Caption = "ToFlow", Content =
@"
**`Pulse.ToFlow(...)`** Executes a subflow over a value or collection.

**Example:**
```csharp
var subFlow =
    from anInt in Pulse.Start<int>()
    from _ in Pulse.Trace(anInt)
    select anInt;
var flow =
    from box in Pulse.Start<Box<int>>()
    from _ in Pulse.ToFlow(subFlow, box.Value) // <=
    select box;
```
")]
    [Fact]
    public void Pulse_to_flow()
    {
        var subFlow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Trace(anInt)
            select anInt;
        var flow =
            from box in Pulse.Start<Box<int>>()
            from _ in Pulse.ToFlow(subFlow, box.Value)
            select box;
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(new Box<int>(42));
        Assert.Equal([42], collector.TheExhibit);
    }

    [Fact]
    public void Pulse_to_flow_collection()
    {
        var subFlow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Trace(anInt + 1)
            select anInt;
        var flow =
            from input in Pulse.Start<List<int>>()
            from _ in Pulse.ToFlow(subFlow, input)
            select input;
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse([41, 41]);
        Assert.Equal([42, 42], collector.TheExhibit);
    }

    [Doc(Order = Chapters.MakeItFlow + "-1", Caption = "", Content =
@"
**`Pulse.ToFlowIf(...)`** Executes a subflow over a value or collection, conditionally.

**Example:**
```csharp
var subFlow =
    from anInt in Pulse.Start<int>()
    from _ in Pulse.Trace(anInt)
    select anInt;
var flow =
    from box in Pulse.Start<Box<int>>()
    from _ in Pulse.ToFlowIf(box.Value != 42, subFlow, () => box.Value) // <=
    select box;
```
")]
    [Fact]
    public void Pulse_to_flow_if()
    {
        var subFlow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Trace(anInt)
            select anInt;
        var flow =
            from input in Pulse.Start<Box<int>>()
            from _ in Pulse.ToFlowIf(input.Value != 42, subFlow, () => input.Value)
            select input;
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(new Box<int>(42));
        signal.Pulse(new Box<int>(7));
        Assert.Single(collector.TheExhibit);
        Assert.Equal(7, collector.TheExhibit[0]);
    }

    [Fact]
    public void Pulse_to_flow_if_collection()
    {
        var subFlow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Trace(input + 1)
            select input;
        var flow =
            from input in Pulse.Start<List<int>>()
            from _ in Pulse.ToFlowIf(true, subFlow, () => input)
            select input;
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse([41, 41]);
        Assert.Equal([42, 42], collector.TheExhibit);
    }

    //     [Fact]
    //     [Doc(Order = Chapters.HowToPulse + "-10", Caption = "Extended `ToFlow` and `ToFlowIf` Overloads", Content =
    // @" These overloads enable more expressive flow definitions, 
    // particularly when working with inline signal construction over values or collections.

    // They are most useful inside **signal definitions**, not in static flow declarations.

    // They allow you to define a short-lived, focused subflow inline, often in combination with `Signal.From(...)` over a function.

    // ## `ToFlow(Func<T, Flow<Unit>>, T)`

    // Applies a flow-producing function to a **single value**. Useful for wrapping a value with a small diagnostic or assertive flow.

    // ```csharp
    // public static Flow<Unit> ToFlow<T>(Func<T, Flow<Unit>> flowFactory, T value)
    // ```

    // ### Example:

    // ```csharp
    // Signal.From<string>(line =>
    //     Pulse.ToFlow(assert => Pulse.Trace(assert), line)
    //         .Then(Pulse.Trace(""✓ done"")))
    //     .SetArtery(WriteData.ToFile())
    //     .Pulse(""hello world"");
    // ```

    // ---

    // ## `ToFlow(Func<T, Flow<Unit>>, IEnumerable<T>)`

    // Applies a flow-producing function to **each value** in a collection.

    // ```csharp
    // public static Flow<Unit> ToFlow<T>(Func<T, Flow<Unit>> flowFactory, IEnumerable<T> values)
    // ```

    // ### Example:

    // ```csharp
    // Signal.From<string[]>(lines =>
    //     Pulse.ToFlow(assert => Pulse.Trace(assert), lines)
    //         .Then(Pulse.Trace(""End of content"")))
    //     .SetArtery(WriteData.ToFile())
    //     .Pulse(lines);
    // ```

    // This pattern is especially valuable for:

    // * Logging/asserting each line
    // * Validating sequences
    // * Flowing structured output with a clear ending
    // ")]
    //     public void Pulse_Flow_Factory()
    //     {
    //         var dotDotDot = Pulse.Trace("...");
    //         var flow =
    //             from anInt in Pulse.Start<int>()
    //             from _ in Pulse.When(anInt == 42, dotDotDot)
    //             select anInt;
    //         var collector = new TheCollector<string>();
    //         Signal.From(flow).SetArtery(collector)
    //             .Pulse(6)
    //             .Pulse(42);
    //         Assert.Equal(["..."], collector.TheExhibit);
    //     }
}