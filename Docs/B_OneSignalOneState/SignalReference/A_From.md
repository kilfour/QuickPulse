# From
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
