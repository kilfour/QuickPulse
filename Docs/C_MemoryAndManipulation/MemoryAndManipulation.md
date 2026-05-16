# Memory And Manipulation
Each signal maintains **gathered cells** (keyed by *type identity*), that store and process specific data types.  
```csharp
static Flow<Flow> flow(Flow _) =>
    from outer in Pulse
        .Prime(() => 1)
        .Trace(a => $"outer: {a}")
    from inner in Pulse.Scoped<int>(a => a + 1,
        Pulse
            .Draw<int>()
            .Trace(a => $"inner: {a}")
            .Manipulate<int>(a => a + 1)
            .Trace(a => $"inner manipulated: {a}"))
    from restored in Pulse
        .Draw<int>()
        .Trace(a => $"restored: {a}")
    select Flow.Continue;
Signal.From<Flow>(flow).Pulse(Flow.Continue);
// Results in => 
//     [ "outer: 1", "inner: 2", "inner manipulated: 3", "restored: 1" ]
```
## Prime: one-time lazy initialization.
`Prime(() => T)` computes and stores a value **once per signal lifetime**.  
## Draw: read from memory.
`Draw<T>()` retrieves the current value from the signal's memory for type `T`.  
The `Draw<TCell, T>(Func<TCell, T> func)` is just a bit of sugar to enable accessing nested values.  
## State aware overloads.
Most `Pulse` methods have one or more utility overloads that combines `.Draw()` functionality
with the overloaded method's functionality.  
It can be seen in the example at the top, but here's another one, showing a more focused usage:  
```csharp
static Flow<Flow> flow(Flow _) =>
    from __ in Pulse.Prime(() => 41)
    from ___ in Pulse.Draw<int>().Trace(a => a + 1)
    select Flow.Continue;
// Pulse() => results in 42
```
## Manipulate: controlled mutation of *primed* state.
`Manipulate<T>(Func<T,T>)` updates the current value of the *gathered cell* for type `T`.  
The return value of `Manipulate` is the **new value**, which can be used immediately in the flow.  
```csharp
static Flow<Flow> flow(int input) =>
    from _1 in Pulse.Prime(() => 0)
    from i in Pulse.Manipulate<int>(x => x + 10) // <= update int cell
    from _2 in Pulse.Trace(i + input)            // <= use the new value
    select Flow.Continue;
Signal.From<int>(flow).Pulse(32);
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
_ =>
   from _1 in Pulse.Prime(() => new Int1(1))
   from _2 in Pulse.Prime(() => new Int2(2))
   from _3 in Pulse.Trace(_1.Number + _2.Number)
   select Flow.Continue;
```
## Postfix Operators
Although the behaviour is logical once you think about it, it can feel a bit unintuitive,
but when using Postfix operators, beware that they return the *old* value.  
```csharp
static Flow<Flow> flow(int input) =>
    from cell in Pulse
        .Prime(() => 0).Dissipate()
        .Manipulate<int>(a => a++).Dissipate()
        .Draw<int>()
    from _ in Pulse.Trace(cell + input)
    select Flow.Continue;
Signal.From<int>(flow).SetArtery(latch).Pulse(41);
// Result => 41. Not 42!
```
Use prefix form or pure expressions instead.  
* **Recommended:** `Pulse.Manipulate<int>(a => a + 1)`  
