## QuickPulse
Do you know how you sometimes leave your house—maybe to get some cigarettes—and start thinking about something?
Your brain takes over.
You walk straight past the shop, and the legs just keep going.
An hour later, you look up, and you're in the next village wondering how you got there.

No? Just me?

Well, okay.

It happens in code too, ... quite a lot.
This library is the result of one of those walks through a dark forest.
And yes, it did *literally* involve Trees.

## Building a Flow
To explain how QuickPulse works (not least to myself), let's build up a flow step by step.

### The Minimal Flow

```csharp
from anInt in Pulse.Start<int>()
select anInt;
```

The type generic in `Pulse.Start<T>` defines the **input type** to the flow.
**Note:** It is required to select the result of `Pulse.Start(...)` at the end of the LINQ chain for the flow to be considered well-formed.

### Doing Something with the Input
Let's trace the values as they pass through:

```csharp
from anInt in Pulse.Start<int>()
from trace in Pulse.Trace(anInt)
select anInt;
```


### Executing a Flow
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


### Sending Values Through the Flow
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


### Capturing the Trace
**Capturing the trace**

To observe what flows through, we can add an `IArtery`.
There are a few ways to do this—here’s one using `SetArtery` directly on the signal.

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



## Signalling
### From

**`Signal.From<T>(Flow<T> flow)`** is a simple factory method used to get hold of a `Signal<T>` instance
that wraps the passed in `Flow<T>`.


**Example:**
```csharp
from anInt in Pulse.Start<int>()
select anInt;
var signal = Signal.From(flow);
```


### Pulse
**`Signal<T>.Pulse(...)`** is the only way a flow can be instructed to do useful work.
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


### Set Artery
**`Signal<T> SetArtery(IArtery artery)`** is used to inject an `IArtery` into the flow.
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` calls will be received by this .

A full example of this can be found at the end of the 'Building a Flow' chapter.


## How To Pulse
### Start

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


## No Where
###  Why There Is No `.Where(...)` in QuickPulse LINQ

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

### Instead of `where`, use:

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

---

### ✅ Why This Matters

Avoiding `.Where(...)` keeps evaluation order predictable and prevents accidental crashes in:

* Diagnostic flows
* Shrinking logic
* Custom combinators and trace sequences

It's a minor trade-off in exchange for greater composability and correctness.


