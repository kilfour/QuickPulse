# QuickPulse
Do you know how you sometimes leave your house—maybe to get some cigarettes—and start thinking about something?
Your brain takes over.
You walk straight past the shop, and the legs just keep going.
An hour later, you look up, and you're in the next village wondering how you got there.

No? Just me?

Well, okay.

It happens in code too, ... quite a lot.
This library is the result of one of those walks through a dark forest.
And yes, it did *literally* involve Trees.

# Building a Flow
To explain how QuickPulse works (not least to myself), let's build up a flow step by step.

## The Minimal Flow

```csharp
from anInt in Pulse.Start<int>()
select anInt;
```

The type generic in `Pulse.Start<T>` defines the **input type** to the flow.
**Note:** It is required to select the result of `Pulse.Start(...)` at the end of the LINQ chain for the flow to be considered well-formed.

## Doing Something with the Input
Let's trace the values as they pass through:

```csharp
from anInt in Pulse.Start<int>()
from trace in Pulse.Trace(anInt)
select anInt;
```


## Executing a Flow
To execute a flow, we need a `Signal<T>`, which is created via:

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


## Sending Values Through the Flow
Once you have a signal, you can push values into the flow by calling:

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


## Capturing the Trace
**Capturing the trace**

To observe what flows through, we can add an `IArtery`.
There are a few ways to do this, here's one using `SetArtery` directly on the signal.

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



# How To Pulse
## Start

**`Pulse.Start()`** is explained in a previous chapter, but for completeness sake, here's a quick recap.

Every flow definition needs to start with a call to `Pulse.Start()`.
This strongly types the values that the flow can receive.
In adition the result of the call needs to be used in the select part of the LINQ expression.
This strongly types the flow itself.

**Example:**
```csharp
from anInt in Pulse.Start<int>() // <=
select anInt;
```


## Using

**`Pulse.Using(...)`** Assigns an `IArtery` to the flow context, and thus enables tracing. 

**Example:**
```csharp
var collector = new TheCollector<int>();
var flow =
    from anInt in Pulse.Start<int>()
    from _ in Pulse.Using(collector) // <= 
    from t in Pulse.Trace(anInt)
    select anInt;
```


## Trace

**`Pulse.Trace(...)`** Emits trace data unconditionally to the current artery.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.Trace(anInt) // <=
select anInt;
```


## TraceIf

**`Pulse.TraceIf(...)`** Emits trace data conditionally, based on a boolean flag.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.TraceIf(anInt != 42, anInt) // <=
select anInt;
```


## Gather

**`Pulse.Gather(...)`** Binds a mutable box into flow memory (first write wins).

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1) // <=
select anInt;
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


## NoOp

**`Pulse.NoOp(...)`** A do-nothing operation (useful for conditional branches). 

**Example:**
```csharp
from anInt in Pulse.Start<int>()
    from _ in Pulse
        .NoOp(/* --- Also useful for Comments --- */)
    select anInt;
```


# Pulsing a Flow: One Signal, One State

In QuickPulse, a `Signal<T>` is more than just a way to push values into a flow;
it's a **stateful conduit**. Each `Signal<T>` instance wraps a specific `Flow<T>` and carries its own **internal state**,
including any `Gather(...)` values or scoped manipulations applied along the way.

When you call `Signal.Pulse(...)`, you're not broadcasting into some shared pipeline,
you're feeding **a single stateful flow machine**,
which responds, remembers, and evolves with each input.

This means:

* You can create **multiple signals** from the same flow definition, each with **independent state**.
* Or, reuse one signal to process a sequence of values, with state accumulating over time.

In short: **one signal, one evolving state**.

```
[ Signal<T> ] ---> [ Flow<T> + internal state ]
       |                    ^
       |                    |
       +---- Pulse(x) ------+
```

This design lets you model streaming behavior, accumulate context, or isolate runs simply by managing signals explicitly.


## From

**`Signal.From(...)`** is a simple factory method used to get hold of a `Signal<T>` instance
that wraps the passed in `Flow<T>`.


**Example:**
```csharp
from anInt in Pulse.Start<int>()
select anInt;
var signal = Signal.From(flow);
```


## Pulse
**`Signal.Pulse(...)`** is the only way a flow can be instructed to do useful work.
**Todo:** *Explain how signal wraps state.*
In its simplest form this looks like the following.
**Example:**
```csharp
from anInt in Pulse.Start<int>()
select anInt;
var signal = Signal.From(flow);
signal.Pulse(42);
```
This sends the int `42` into the flow.


The argument of this method is actually `params T[] input`, so you can send multiple values in, in one call.
**Example:**
```csharp
signal.Pulse(42, 43, 44);
```
This will execute the flow three times, once for each value passed in.


For ease of use, when dealing with `IEnumerable` return values from various sources,
an overload exists: `Pulse(IEnumerable<T> inputs)`. 
**Example:**
```csharp
signal.Pulse(new List<int> { 42, 43, 44 });
```
This behaves exactly like the previous example.


## Set Artery
**`Signal.SetArtery(...)`** is used to inject an `IArtery` into the flow.
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` calls will be received by this .

A full example of this can be found at the end of the 'Building a Flow' chapter.


## Manipulate
**`Signal.Manipulate(...)`** is used in conjunction with `Pulse.Gather(...)`,
and allows for manipulating the flow in between pulses.
**Given this setup:**
```csharp
 var flow =
    from anInt in Pulse.Start<int>()
    from gathered in Pulse.Gather(0)
    from _ in Pulse.Trace($"{anInt} : {gathered.Value}")
    select anInt;
var signal = Signal.From(flow);
```
And we pulse once like so : `signal.Pulse(42);` the flow will gather the input in the gathered range variable and
trace output is : `42 : 0`.

If we then call `Manipulate` like so: `signal.Manipulate<int>(a => a + 1);`, the next pulse: `signal.Pulse(42);`,
produces `42 : 1`.



## Scoped
**`Signal.Scoped(...)`** is sugaring for 'scoped' usage of the `Manipulate` method.

Given the same setup as before, we can write :

```csharp
signal.Pulse(42);
using (signal.Scoped<int>(a => a + 1, a => a - 1))
{
    signal.Pulse(42);
}
signal.Pulse(42);
```
And the trace values will be:
```
42 : 0
42 : 1
42 : 0
```


# No Where
## Why There Is No `.Where(...)` in QuickPulse LINQ
In standard LINQ-to-objects, the `where` clause is lazily applied and safely filters values *before* any downstream computation happens. This works because `IEnumerable<T>` defers evaluation until iteration.

But **QuickPulse uses monadic LINQ over computation flows** (`Flow<T>`), not sequences. In monadic LINQ, the C# compiler desugars `where` **after** any preceding `let`, `from`, or `select` clauses — and **evaluates them eagerly**.

This means:

```csharp
from x in Flow<T>
where x != null
let y = x.SomeProperty // NRE: still evaluated even if x is null!
```

The `let` runs *before* the `where`, causing runtime exceptions — even though it looks safe.

---

## Instead of `where`, use:

* `Pulse.TraceIf(...)`
* `Pulse.NoOp()`
* Custom `.If(...)` / `.Guard(...)` combinators
* Plain ternary logic inside `SelectMany` chains

Example:

```csharp
from diag in Pulse.Start<DiagnosticInfo>()
from _ in diag.Tags.Contains("Phase")
    ? Pulse.Trace("...")
    : Pulse.NoOp()
```


## Why This Matters

Avoiding `.Where(...)` keeps evaluation order predictable and prevents accidental crashes in:

* Diagnostic flows
* Shrinking logic
* Custom combinators and trace sequences

It's a minor trade-off in exchange for greater composability and correctness.


