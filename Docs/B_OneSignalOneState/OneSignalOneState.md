# Pulsing a Flow: One Signal, One State

In QuickPulse, a `Signal<T>` is more than just a way to push values into a flow;
it's a **stateful conduit**. Each `Signal<T>` instance wraps a specific `Flow<T>` and carries its own **internal state**,
including any `Prime(...)` values or scoped manipulations applied along the way.

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

This design lets you model streaming behaviour, accumulate context, or isolate runs simply by managing signals explicitly.
  
## From
`Signal.From(...)` is a simple factory method used to get hold of a `Signal<T>` instance
that wraps the passed in `Flow<T>`.  
```csharp
var flow =
    from anInt in Pulse.Start<int>()
    select anInt;
var signal = Signal.From(flow);
```
`Signal.From<T>(Func<T, Flow<Flow>>` is a useful overload that allows for inlining simple flows upon Signal creation.  
```csharp
var signal = Signal.From<int>(a => Pulse.Trace(a));
```
## Pulse
### Pulsing One Value
`Signal.Pulse(...)` is the main way a flow can be instructed to do useful work.  
```csharp
Signal.From<int>(a => Pulse.Trace(a))
    .Pulse(42)
    .Pulse(43)
    .Pulse(44);
```
This sends the int's `42`, `43` and `44` into the flow.  
### Pulsing Many Values
For ease of use, when dealing with `IEnumerable` return values from various sources, an overload exists: `Signal.Pulse(IEnumerable<T> inputs)`.   
```csharp
Signal.From<int>(a => Pulse.Trace(a))
    .Pulse([42, 43, 44]);
```
Same behaviour as the single pulse example.  
### Pulsing Nothing
Lastly, in some rare circumstances, a flow does not take any input. In `QuickPulse` *nothing* is represented by a `Flow` type.  
So in order to advance a flow of type `Flow<Flow>` you can use the `Signal.Pulse()` overload.  
```csharp
var flow =
    from _ in Pulse.Start<Flow>()
    from _1 in Pulse.Prime(() => 42)
    from _2 in Pulse.Trace<int>(a => a)
    from _3 in Pulse.Manipulate<int>(a => a + 1)
    select Flow.Continue;
Signal.From(flow)
    .Pulse()
    .Pulse()
    .Pulse();
// This one also results in [42, 43, 44]
```
## Flatline
`Signal.FlatLine(...)` is a terminal operation that runs a final flow once the main signal has completed pulsing.
It's useful for summarizing, tracing, or cleaning up after a sequence of pulses.  

The following example does use some features fully explained in the chapter **'Memory And Manipulation'**.  
```csharp
var flow =
    from _ in Pulse.Start<Flow>()
    from __ in Pulse.Prime(() => 0)
    from ___ in Pulse.Manipulate<int>(a => a + 1)
    select Flow.Continue;
Signal.From(flow)
    .Pulse().Pulse().Pulse()
    .FlatLine(Pulse.Trace<int>(a => a));
// Results in => 3
```
