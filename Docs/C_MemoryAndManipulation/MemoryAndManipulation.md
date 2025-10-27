# Memory And Manipulation
> How QuickPulse remembers, updates, and temporarily alters state.

Each signal maintains **gathered cells** (keyed by *type identity*), think of them as the signal's **internal organs**
that store and process specific data types.
Just as your heart handles blood and lungs handle air, each gathered cell specializes in a particular data type.
  
```csharp
var flow =
    from _ in Pulse.Start<Flow>()
    from _1 in Pulse.Prime(() => 1)
    from _2 in Pulse.Trace<int>(a => $"outer: {a}")
    from _3 in Pulse.Scoped<int>(a => a + 1,
        from __1 in Pulse.Trace<int>(a => $"inner: {a}")
        from __2 in Pulse.Manipulate<int>(a => a + 1)
        from __3 in Pulse.Trace<int>(a => $"inner manipulated: {a}")
        select Flow.Continue)
    from _4 in Pulse.Trace<int>(a => $"restored: {a}")
    select Flow.Continue;
Signal.From(flow).Pulse(Flow.Continue);
// Results in => 
//     [ "outer: 1", "inner: 2", "inner manipulated: 3", "restored: 1" ]
```
## Prime: one-time lazy initialization.
`Prime(() => T)` computes and stores a value **once per signal lifetime**.  
## Draw: read from memory.
`Draw<T>()` retrieves the current value from the signal's memory for type `T`.  
The `Draw<TBox, T>(Func<TBox, T> func)` is just a bit of sugar to enable accessing nested values.  
## State aware overloads.
Most `Pulse` methods have one or more utility overloads that combines `.Draw()` functionality
with the overloaded method's functionality.  
It can be seen in the example at the top, but here's another one, showing a more focused usage:  
```csharp
var flow =
    from _ in Pulse.Start<Flow>()
    from __ in Pulse.Prime(() => 41)
    from ___ in Pulse.Trace<int>(a => a + 1)
    select Flow.Continue;
// Pulse() => results in 42
```
## Manipulate: controlled mutation of *primed* state.
`Manipulate<T>(Func<T,T>)` updates the current value of the *gathered cell* for type `T`.  
The return value of `Manipulate` is the **new value**, which can be used immediately in the flow.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 0)
    from i in Pulse.Manipulate<int>(x => x + 10) // <= update int cell
    from _2 in Pulse.Trace(i + input)            // <= use the new value
    select input;
Signal.From(flow).Pulse(32);
```
## Scoped: temporary overrides with automatic restore.
`Scoped<T>(enter, innerFlow)` runs `innerFlow` with a **temporary** value for the *gathered cell* of type `T`. On exit, the outer value is restored.  
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
 from _ in Pulse.Start<Flow>()
       from _1 in Pulse.Prime(() => new Int1(1))
       from _2 in Pulse.Prime(() => new Int2(2))
       from _3 in Pulse.Trace(_1.Number + _2.Number)
       select Flow.Continue;
```
## Postfix Operators
Although the behaviour is logical once you think about it, it can feel a bit unintuitive,
but when using Postfix operators, beware that they return the *old* value.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.Prime(() => 0)
    from __ in Pulse.Manipulate<int>(a => a++) // <= int is still 0 in memory cell
    from now in Pulse.Trace<int>(a => a + input)
    select input;
Signal.From(flow).Pulse(41);
// Result => 41. Not 42!
```
Use prefix form or pure expressions instead.  
* **Recommended:** `Pulse.Manipulate<int>(a => a + 1)`  
