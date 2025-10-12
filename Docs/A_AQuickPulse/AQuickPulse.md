# A Quick Pulse
To explain how QuickPulse works (not least to myself), let's build up a flow step by step.  
## The Minimal Flow
The type generic in `Pulse.Start<T>` defines the **input type** to the flow.  
**Note:** It is required to select the result of `Pulse.Start(...)` at the end of the LINQ chain for the flow to be considered well-formed.  
```csharp
    from anInt in Pulse.Start<int>()
    select anInt;
```
### A Mental Map
> See the river through the flows.

Before diving deeper, it helps to understand the three pillars that make up QuickPulse's core.

1. `Flow<T>`: 
A flow is a *recipe for behaviour*. It defines how input values are transformed, traced, or manipulated.
You can think of it as the *program*, declarative, reusable, and testable.
A flow itself doesn't do anything until it's pulsed.

2. `Signal<T>`: 
A signal is a *living instance* of a flow. It remembers, reacts, and evolves with each pulse.
Every signal carries its own internal state and provides the entry point for execution.
You can have multiple signals from the same flow, each one a separate, independent life.

3. `IArtery`: 
Arteries are the *output channels* of a signal. They collect, display, or record whatever the flow emits,
be it traces, diagnostics, or effects.
Arteries make flows observable, enabling introspection, debugging, and persistence.  
## Doing Something with the Input
Let's trace the values as they pass through:  
```csharp
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;
```
## Executing a Flow
To execute a flow, we need a `Signal<T>`, which is created via: `Signal.From<T>(Flow<T> flow)`.

Example:  
```csharp
var flow =
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;
var signal = Signal.From(flow);
```
## Sending Values Through the Flow
Once you have a signal, you can push values into the flow by calling: `Signal.Pulse(...)`.

For example, sending the value `42` into the flow:  
```csharp
 Signal.From(
        from anInt in Pulse.Start<int>()
        from trace in Pulse.Trace(anInt)
        select anInt)
    .Pulse(42);
```
## Capturing the Trace
To observe what flows through, we can add an `IArtery` by using `SetArtery` directly on the signal.

Example:  
```csharp
public void Adding_an_artery()
{
    var collector = TheCollector.Exhibits<int>();
    Signal.From(
            from anInt in Pulse.Start<int>()
            from trace in Pulse.Trace(anInt)
            select anInt)
        .SetArtery(collector)
        .Pulse([42, 43, 44]);
    Assert.Equal(3, collector.TheExhibit.Count);
    Assert.Equal(42, collector.TheExhibit[0]);
    Assert.Equal(43, collector.TheExhibit[1]);
    Assert.Equal(44, collector.TheExhibit[2]);
}
```
