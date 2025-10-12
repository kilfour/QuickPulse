# Bending The River
> About knees and meanders.

This chapter explores and explains the various ways you can control the execution flow of an ... erm ... `Flow`.  
It is more of a reference chapter and more prone to changes, than any of the other chapters.
This area is still evolving, expect simplifications.

All flow examples below will be executed using the following `Signal`:  
```csharp
Signal.From(flow)
    .SetArtery(TheCollector.Exhibits<string>())
    .Pulse([1, 2, 3, 4, 5])
    .GetArtery<Collector<string>>()
    .TheExhibit;
```
In addition, unless explicitly specified the result of executing the example flow will always be:   
```bash
even
even
```
## Using a Ternary Conditional Operator (*If/Then/Else*)
```csharp
    from input in Pulse.Start<int>()
    let conditional =
        input % 2 == 0
        ? Pulse.Trace("even")
        : Pulse.Trace("uneven")
    from _ in conditional
    select input;
```
**Result:**  
```bash
uneven
even
uneven
even
uneven
```
> Prefer Pulse.NoOp() when you want an If/Then without an else-branch:  
```csharp
    from input in Pulse.Start<int>()
    let conditional =
        input % 2 == 0
        ? Pulse.Trace("even")
        : Pulse.NoOp()
    from _ in conditional
    select input;
```
*Note:* While the ternary operator works, QuickPulse provides more idiomatic ways to deal with conditional statemens, which we will look at below.  
## When
```csharp
    from input in Pulse.Start<int>()
    from _ in Pulse.When(input % 2 == 0, Pulse.Trace("even"))
    select input;
```
### Flow Factory Version
```csharp
    from input in Pulse.Start<int>()
    from _ in Pulse.When(input % 2 == 0, () => Pulse.Trace("even"))
    select input;
```
### Using State
```csharp
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 2)
    from _2 in Pulse.When<int>(a => input % a == 0, Pulse.Trace("even"))
    select input;
```
### Using State: Flow Factory Version
```csharp
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 2)
    from _2 in Pulse.When<int>(a => input % a == 0, () => Pulse.Trace("even"))
    select input;
```
> **Why the factory overloads?**  
> `When(..., () => flow)` and many other `Pulse` methods using the factory flow pattern defer creating the subflow/value until the condition is true, avoiding work and allocations on the false path.  
## TraceIf
```csharp
    from input in Pulse.Start<int>()
    from _ in Pulse.TraceIf(input % 2 == 0, () => "even")
    select input;
```
### Using State
```csharp
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 2)
    from _2 in Pulse.TraceIf<int>(a => input % a == 0, () => "even")
    select input;
```
### Using State, ... Twice
```csharp
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 2)
    from _2 in Pulse.TraceIf<int>(a => input % a == 0, a => $"{a}:even")
    select input;
```
**Result:**  
```bash
2:even
2:even
```
