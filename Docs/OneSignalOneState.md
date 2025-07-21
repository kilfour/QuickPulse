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

## Tracing

**`Signal.Tracing<T>()`** is sugaring for: 
```csharp
var flow =
    from start in Pulse.Start<T>()
    from _ in Pulse.Trace(start)
    select start;
return new Signal<T>(flow);
```
**Example:**
```csharp
Signal.Tracing<string>();
```
Useful if you want to just quickly grab a tracer.

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

## Pulse Multiple
**`Signal.PulseMultiple(...)`** is a helper method that sugars a `for(int i = ...)` type structure.

**Example:**
```csharp
var collector = new TheCollector<int>();
var flow =
    from anInt in Pulse.Start<int>()
    from g in Pulse.Gather(0)
    from t in Pulse.Trace(anInt + g.Value)
    from e in Pulse.Effect(() => g.Value++)
    select anInt;
var signal = Signal.From(flow).SetArtery(collector);
signal.PulseMultiple(3, 39);
```
Trace output: `40, 41, 42`.

## Pulse Until
**`Signal.PulseUntil(...)`** is a helper method that sugars a `while(...)` type structure.

**Example:**
```csharp
var collector = new TheCollector<int>();
var flow =
    from anInt in Pulse.Start<int>()
    from g in Pulse.Gather(0)
    from t in Pulse.Trace(anInt + g.Value)
    from e in Pulse.Effect(() => g.Value++)
    select anInt;
var signal = Signal.From(flow).SetArtery(collector);
signal.PulseUntil(() => collector.TheExhibit.Contains(42), 39);
```
Trace output: `40, 41, 42`.

**Warning:** Make sure you stop pulsing. `Signal.PulseUntil(...)` throws an exception if you try to pulse over 256 times.

## Pulse Multiple Until
**`Signal.PulseMultipleUntil(...)`** is a combination of the previous two methods.
Pulses N amount of times, N being the method's first parameter.  

**Example:**
```csharp
var collector = new TheCollector<int>();
var flow =
    from anInt in Pulse.Start<int>()
    from g in Pulse.Gather(0)
    from t in Pulse.Trace(anInt + g.Value)
    from e in Pulse.Effect(() => g.Value++)
    select anInt;
var signal = Signal.From(flow).SetArtery(collector);
signal.PulseMultipleUntil(3, () => false, 40);
```
Trace output: `40, 41, 42`.

But if the condition supplied is satisfied it will stop pulsing early.  

**Example:**
```csharp
var collector = new TheCollector<int>();
var flow =
    from anInt in Pulse.Start<int>()
    from g in Pulse.Gather(0)
    from t in Pulse.Trace(anInt + g.Value)
    from e in Pulse.Effect(() => g.Value++)
    select anInt;
var signal = Signal.From(flow).SetArtery(collector);
signal.PulseMultipleUntil(3, () => false, 40);
```
Trace output: `40, 41, 42`.

## Set Artery
**`Signal.SetArtery(...)`** is used to inject an `IArtery` into the flow.
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` calls will be received by this .

A full example of this can be found at the end of the 'Building a Flow' chapter.

## Set And Return Artery
**`Signal.SetAndReturnArtery(...)`** is the same as above, but instead of returning the signal it returns the artery.
```csharp
var collector = signal.SetAndReturnArtery(new TheCollector<int>());
```

## Get Artery
**`Signal.GetArtery<TArtery>(...)`** can be used to retrieve the current `IArtery` set on the signal.
**Example:**
```csharp
var signal = Signal.Tracing<int>().SetArtery(new TheCollector<int>()).Pulse(42);

var collector = signal.GetArtery<TheCollector<int>>()!;
Assert.Single(collector.TheExhibit);
Assert.Equal(42, collector.TheExhibit[0]);
```

**`Signal.GetArtery<TArtery>(...)`** throws if no `IArtery` is currently set on the `Signal`.

**`Signal.GetArtery<TArtery>(...)`** throws if trying to retrieve the wrong type of `IArtery`.

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
And we pulse once like so: `signal.Pulse(42);` the flow will gather the input in the gathered range variable and
trace output is: `42 : 0`.

If we then call `Manipulate` like so: `signal.Manipulate<int>(a => a + 1);`, the next pulse: `signal.Pulse(42);`,
produces `42 : 1`.  


**Warning:** `Manipulate` mutates state between pulses. Sharp tool, like a scalpel.
Don't cut yourself.

## Scoped
**`Signal.Scoped(...)`** is sugaring for 'scoped' usage of the `Manipulate` method.

Given the same setup as before, we can write:

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
**Warning:** `Scoped` Temporarily alters state.  
Like setting a trap, stepping into it, and then dismantling it.  
Make sure you spring it though.

## ToFile
**`Signal.ToFile<T>(string? maybeFileName = null)`** is shorthand for:

`Signal.Tracing<T>().SetArtery(WriteData.ToFile(string? maybeFileName = null))

This allows quick logging of all values flowing through the signal to a file.
