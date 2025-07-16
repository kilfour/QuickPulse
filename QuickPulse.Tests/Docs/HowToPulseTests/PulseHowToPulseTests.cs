using QuickExplainIt;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace QuickPulse.Tests.Docs.HowToPulseTests;


[Doc(Order = Chapters.HowToPulse, Caption = "How To Pulse", Content =
@"**Cheat Sheet:**

| Combinator         | Role / Purpose                                                                |
| ------------------ | ----------------------------------------------------------------------------- |
| **Start<T>()**     | Starts a new flow. Defines the input type.                                    |
| **Using(...)**     | Applies an `IArtery` to the flow context, enables tracing.                    |
| **Trace(...)**     | Emits trace data unconditionally to the current artery.                       |
| **TraceIf(...)**   | Emits trace data conditionally, based on a boolean flag.                      |
| **Effect(...)**    | Performs a side-effect (logging, mutation, etc.) without yielding a value.    |
| **EffectIf(...)**  | Performs a side-effect conditionally.                                         |
| **Gather<T>(...)** | Captures a mutable box into flow memory (first write wins).                   |
| **ToFlow(...)**    | Invokes a subflow over a value or collection.                                 |
| **ToFlowIf(...)**  | Invokes a subflow conditionally, using a supplier for the input.              |
| **NoOp()**         | Applies a do-nothing operation (for conditional branches or comments).        |

")]
public class PulseHowToPulseTests
{
    [Doc(Order = Chapters.HowToPulse + "-1", Caption = "Start", Content =
@"
**`Pulse.Start()`** is explained in a previous chapter, but for completeness sake, here's a quick recap.

Every flow definition needs to start with a call to `Pulse.Start()`.
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

    //     [Doc(Order = Chapters.HowToPulse + "-2", Caption = "Using", Content =
    // @"
    // **`Pulse.Using(...)`** Assigns an `IArtery` to the flow context, and thus enables tracing. 

    // **Example:**
    // ```csharp
    // var collector = new TheCollector<int>();
    // var flow =
    //     from anInt in Pulse.Start<int>()
    //     from _ in Pulse.Using(collector) // <= 
    //     from t in Pulse.Trace(anInt)
    //     select anInt;
    // ```
    // ")]
    //     [Fact]
    //     public void Pulse_using()
    //     {
    //         var collector = new TheCollector<int>();
    //         var flow =
    //             from anInt in Pulse.Start<int>()
    //             from _ in Pulse.Using(collector)
    //             from t in Pulse.Trace(anInt) // <= 
    //             select anInt;
    //         var signal = Signal.From(flow);
    //         signal.Pulse(42);
    //         Assert.Single(collector.TheExhibit);
    //         Assert.Equal(42, collector.TheExhibit[0]);
    //     }

    [Doc(Order = Chapters.HowToPulse + "-3", Caption = "Trace", Content =
@"
**`Pulse.Trace(...)`** Emits trace data unconditionally to the current artery.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.Trace(anInt) // <=
select anInt;
```
")]
    [Fact]
    public void Pulse_trace()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Trace(anInt)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.HowToPulse + "-4", Caption = "TraceIf", Content =
@"
**`Pulse.TraceIf(...)`** Emits trace data conditionally, based on a boolean flag.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.TraceIf(anInt != 42, anInt) // <=
select anInt;
```
")]
    [Fact]
    public void Pulse_trace_if()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.TraceIf(anInt != 42, anInt)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        signal.Pulse(7);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(7, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.HowToPulse + "-5", Caption = "Gather", Content =
@"
**`Pulse.Gather(...)`** Binds a mutable box into flow memory (first write wins).

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1) // <=
select anInt;
```
**Warning:** `Gather` is thread-hostile by design, like a chainsaw. Use accordingly.  
Useful, powerful, and absolutely the wrong tool to wield in a multithreaded environment.
")]
    [Fact]
    public void Pulse_gather()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from box in Pulse.Gather(1)
            from _ in Pulse.Trace(anInt + box.Value)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(41);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.HowToPulse + "-6", Caption = "Effect", Content =
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

    [Doc(Order = Chapters.HowToPulse + "-7", Caption = "EffectIf", Content =
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
            from _ in Pulse.TraceIf(seen42.Value, anInt)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(40);
        signal.Pulse(42);
        signal.Pulse(7);
        Assert.Equal(2, collector.TheExhibit.Count);
        Assert.Equal(42, collector.TheExhibit[0]);
        Assert.Equal(7, collector.TheExhibit[1]);
    }

    [Doc(Order = Chapters.HowToPulse + "-8", Caption = "ToFlow", Content =
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
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.HowToPulse + "-9", Caption = "ToFlowIf", Content =
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
            from box in Pulse.Start<Box<int>>()
            from _ in Pulse.ToFlowIf(box.Value != 42, subFlow, () => box.Value)
            select box;
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(new Box<int>(42));
        signal.Pulse(new Box<int>(7));
        Assert.Single(collector.TheExhibit);
        Assert.Equal(7, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.HowToPulse + "-10", Caption = "NoOp", Content =
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
    [Fact]
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