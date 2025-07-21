# Number One, Make it Flow
**Cheat Sheet:**

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

## Start

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

## Trace

**`Pulse.Trace(...)`** emits trace data unconditionally to the current artery.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.Trace(anInt) // <=
select anInt;
```

## TraceIf

**`Pulse.TraceIf(...)`** emits trace data conditionally, based on a boolean flag.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.TraceIf(anInt != 42, () => anInt) // <=
select anInt;
```

## FirstOf

**`Pulse.FirstOf(...)`** runs the first flow in a sequence of (condition, flow) pairs where the condition evaluates to true.

**Example:**
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.TraceFirstOf(
        (() => input == 42, () => Pulse.Trace("answer")),
        (() => input == 666, () => Pulse.Trace("beëlzebub")),
        (() => input == 42 || input == 666, () => Pulse.Trace("never")))
    select input;
```

## Gather
**`Pulse.Gather(...)`** Binds a mutable box into flow memory (first write wins).

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1) // <=
select anInt;
```
**`Pulse.Gather<T>()`** used without an argument, serves as a 'getter' of a previously gathered value.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1)
from val in Pulse.Gather<int>() // <=
from _ in Pulse.Trace(anInt + val.Value)
select anInt;
```
**`Pulse.Gather<T>()`** throws if no value of the requested type is available.
## Scoped
**`Pulse.Scoped<T>(...)`** temporarily alters gathered state of type T, runs an inner flow,
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
```
## Effect

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

## EffectIf

**`Pulse.EffectIf(...)`** Same as above, but conditional. 

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from seen42 in Pulse.Gather(false)
from eff in Pulse.EffectIf(anInt == 42, () => seen42.Value = true) // <=
select anInt;
```

## ToFlow

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

## ToFlowIf

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

## When

**`Pulse.When(...)`** Executes a subflow conditionally.

A flow that does not take an input like `var someMessage = Pulse.Trace("Some Message")` can be defined as a sub flow,
and executed by simple including it in the Linq chain: `from _ in someMessage`.

If we want to flow, based on a predicate, we could do: `from _ in predicate ? someMessage : Pulse.NoOp()`.

Which is fine but with `Pulse.When(...)` we can do better.

**Example:**
```csharp
var dotDotDot = Pulse.Trace("...");
var flow =
    from anInt in Pulse.Start<int>()
    from _ in Pulse.When(anInt == 42, dotDotDot) // <=
    select anInt;
var collector = new TheCollector<string>();
Signal.From(flow).SetArtery(collector)
    .Pulse(6)
    .Pulse(42);
Assert.Equal(["..."], collector.TheExhibit);
```

## NoOp

**`Pulse.NoOp(...)`** A do-nothing operation (useful for conditional branches). 

**Example:**
```csharp
from anInt in Pulse.Start<int>()
    from _ in Pulse
        .NoOp(/* --- Also useful for Comments --- */)
    select anInt;
```

