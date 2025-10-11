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
**`Signal.Pulse(...)`** is the main way a flow can be instructed to do useful work.
In its simplest form this looks like the following.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
select anInt;
var signal = Signal.From(flow);
signal.Pulse(42);
```
This sends the int `42` into the flow.
  
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
  
## Set And Return Artery
**`Signal.SetAndReturnArtery(...)`** is the same as above, but instead of returning the signal it returns the artery.
```csharp
var collector = signal.SetAndReturnArtery(TheCollector.Exhibits<int>());
```
  
## Get Artery
**`Signal.GetArtery<TArtery>(...)`** can be used to retrieve the current `IArtery` set on the signal.
**Example:**
```csharp
var signal = Signal.From<int>(a => Pulse.Trace(a)).SetArtery(TheCollector.Exhibits<int>()).Pulse(42);

var collector = signal.GetArtery<TheCollector<int>>()!;
Assert.Single(collector.TheExhibit);
Assert.Equal(42, collector.TheExhibit[0]);
```
  
**`Signal.GetArtery<TArtery>(...)`** throws if trying to retrieve the wrong type of `IArtery`.
  
