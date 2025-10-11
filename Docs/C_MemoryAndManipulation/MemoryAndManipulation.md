# Memory And Manipulation
> How QuickPulse remembers, updates, and temporarily alters state.  
```csharp
var seen = TheCollector.Exhibits<string>();
var flow =
    from input in Pulse.Start<Unit>()
    from _1 in Pulse.Prime(() => 1)
    from _2 in Pulse.Trace<int>(a => $"outer: {a}")
    from _3 in Pulse.Scoped<int>(a => a + 1,
        from __1 in Pulse.Trace<int>(a => $"inner: {a}")
        from __2 in Pulse.Manipulate<int>(a => a + 1)
        from __3 in Pulse.Trace<int>(a => $"inner manipulated: {a}")
        select Unit.Instance)
    from _4 in Pulse.Trace<int>(a => $"restored: {a}")
    select Unit.Instance;
Signal.From(flow).SetArtery(seen).Pulse(Unit.Instance);
Assert.Equal(new object[] { "outer: 1", "inner: 2", "inner manipulated: 3", "restored: 1" }, seen.TheExhibit);
```
## Prime: one-time lazy initialization.
`Prime(() => T)` computes and stores a value **once per signal lifetime**.  
## Draw: read from memory.
`Draw<T>()` retrieves the current value from the signal's memory for type `T`.  
## Manipulate: controlled mutation of *primed* state.
`Manipulate<T>(Func<T,T>)` updates the current value of the gathered cell for type `T`.  
## Scoped: temporary overrides with automatic restore.
`Scoped<T>(enter, innerFlow)` runs `innerFlow` with a **temporary** value for the gathered cell of type `T`. On exit, the outer value is restored.  
Any `Manipulate<T>` inside the scope affects the **scoped** value and is discarded on exit.  
## Type Identity Matters
Use wrapper records to keep multiple cells of the same underlying type.    
```csharp
public record Int1(int Number) { }
```
```csharp
public record Int2(int Number) { }
```
```csharp
 from start in Pulse.Start<Unit>()
       from _1 in Pulse.Prime(() => new Int1(1))
       from _2 in Pulse.Prime(() => new Int2(2))
       from _3 in Pulse.Trace(_1.Number + _2.Number)
       select start;
```
