using QuickPulse.Explains;
using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse.Tests.Docs.NumberOneMakeItFlow;


[Doc(Order = Chapters.HowToPulse, Caption = "Number One, Make it Flow", Content =
@"**Cheat Sheet:**

| Combinator            | Role / Purpose                                                                |
| --------------------- | ----------------------------------------------------------------------------- |
| **Start<T>()**        | Starts a new flow. Defines the input type.                                    |
| **Using(...)**        | Applies an `IArtery` to the flow context, enables tracing.                    |
| **Trace(...)**        | Emits trace data unconditionally to the current artery.                       |
| **TraceIf(...)**      | Emits trace data conditionally, based on a boolean flag.                      |
| **FirstOf(...)**      | Executes the first flow where its condition is `true`, skips the rest.        |
| **Effect(...)**       | Performs a side-effect (logging, mutation, etc.) without yielding a value.    |
| **EffectIf(...)**     | Performs a side-effect conditionally.                                         |
| **Gather<T>(...)**    | Captures a mutable box into flow memory (first write wins).                   |
| **Scoped<T>(...)**    | Temporarily mutates gathered state during a subflow, then restores it.        |
| **ToFlow(...)**       | Invokes a subflow over a value or collection.                                 |
| **ToFlowIf(...)**     | Invokes a subflow conditionally, using a supplier for the input.              |
| **When(...)**         | Executes the given flow only if the condition is true, without input.         |
| **NoOp()**            | Applies a do-nothing operation (for conditional branches or comments).        |
")]
public class MakeItFlowTests
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
**`Pulse.Trace(...)`** emits trace data unconditionally to the current artery.

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
**`Pulse.TraceIf(...)`** emits trace data conditionally, based on a boolean flag.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.TraceIf(anInt != 42, () => anInt) // <=
select anInt;
```
")]
    [Fact]
    public void Pulse_trace_if()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.TraceIf(anInt != 42, () => anInt)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        signal.Pulse(7);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(7, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.HowToPulse + "-4.5", Caption = "FirstOf", Content =
@"
**`Pulse.FirstOf(...)`** runs the first flow in a sequence of (condition, flow) pairs where the condition evaluates to true.

**Example:**
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.TraceFirstOf(
        (() => input == 42, () => Pulse.Trace(""answer"")),
        (() => input == 666, () => Pulse.Trace(""beëlzebub"")),
        (() => input == 42 || input == 666, () => Pulse.Trace(""never"")))
    select input;
```
")]
    [Fact]
    public void Pulse_FirstOf()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.FirstOf(
                (() => input == 42, () => Pulse.Trace("answer")),
                (() => input == 666, () => Pulse.Trace("beëlzebub")),
                (() => input == 42 || input == 666, () => Pulse.Trace("never")))
            select input;
        var collector = new TheCollector<string>();
        Signal.From(flow).SetArtery(collector).Pulse(42, 666);
        Assert.Equal(["answer", "beëlzebub"], collector.TheExhibit);
    }

    [Doc(Order = Chapters.HowToPulse + "-5", Caption = "Gather", Content =
@"**`Pulse.Gather(...)`** Binds a mutable box into flow memory (first write wins).

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1) // <=
select anInt;
```")]
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

    [Doc(Order = Chapters.HowToPulse + "-5-1", Caption = "", Content =
@"**`Pulse.Gather<T>()`** used without an argument, serves as a 'getter' of a previously gathered value.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1)
from val in Pulse.Gather<int>() // <=
from _ in Pulse.Trace(anInt + val.Value)
select anInt;
```")]
    [Fact]
    public void Pulse_gather_empty_param()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from box in Pulse.Gather(1)
            from val in Pulse.Gather<int>()
            from _ in Pulse.Trace(anInt + val.Value)
            select anInt;
        Signal.From(flow).SetArtery(collector).Pulse(41);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.HowToPulse + "-5-1", Caption = "", Content =
 @"**`Pulse.Gather<T>()`** throws if no value of the requested type is available.")]
    [Fact]
    public void Pulse_gather_empty_param_no_value_throws()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            from val in Pulse.Gather<int>()
            from _ in Pulse.Trace(anInt + val.Value)
            select anInt;
        var ex = Assert.Throws<ComputerSaysNo>(() => Signal.From(flow).Pulse(42));
        Assert.Equal("No value of type Int32 found.", ex.Message);
    }

    [Doc(Order = Chapters.HowToPulse + "-5.1", Caption = "Scoped", Content =
@"**`Pulse.Scoped<T>(...)`** temporarily alters gathered state of type T, runs an inner flow,
and reverts the state after.
**Example:**
```csharp
var collector = new TheCollector<int>();
var innerFlow =
    from anInt in Pulse.Start<int>()
    from scopedBox in Pulse.Gather<int>()
    from _ in Pulse.Trace(anInt + scopedBox.Value)
    select anInt;
var flow =
    from anInt in Pulse.Start<int>()
    from box in Pulse.Gather(0)
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
        var collector = new TheCollector<int>();
        var innerFlow =
            from anInt in Pulse.Start<int>()
            from scopedBox in Pulse.Gather<int>()
            from _ in Pulse.Trace(anInt + scopedBox.Value)
            select anInt;
        var flow =
            from anInt in Pulse.Start<int>()
            from box in Pulse.Gather(0)
            from _ in Pulse.Trace(anInt + box.Value)
            from scopeInt in Pulse.Scoped<int>(
                a => a + 1,
                Pulse.ToFlow(innerFlow, anInt))
            from __ in Pulse.Trace(anInt + box.Value)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        Assert.Equal([42, 43, 42], collector.TheExhibit);
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
            from _ in Pulse.TraceIf(seen42.Value, () => anInt)
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


    [Fact]
    [Doc(Order = Chapters.HowToPulse + "-10", Caption = "When", Content =
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
        var collector = new TheCollector<string>();
        Signal.From(flow).SetArtery(collector)
            .Pulse(6)
            .Pulse(42);
        Assert.Equal(["..."], collector.TheExhibit);
    }

    [Fact]
    [Doc(Order = Chapters.HowToPulse + "-11", Caption = "NoOp", Content =
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
        var collector = new TheCollector<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse(42);
        Assert.Empty(collector.TheExhibit);
    }
}