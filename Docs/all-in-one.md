## A Quick Pulse
To explain how QuickPulse works (not least to myself), let's build up a flow step by step.  
### The Minimal Flow
The type generic in `Pulse.Start<T>` defines the **input type** to the flow.  
**Note:** It is required to select the result of `Pulse.Start(...)` at the end of the LINQ chain for the flow to be considered well-formed.  
```csharp
    from anInt in Pulse.Start<int>()
    select anInt;
```
#### A Mental Map
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
### Doing Something with the Input
Let's trace the values as they pass through:  
```csharp
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;
```
### Executing a Flow
To execute a flow, we need a `Signal<T>`, which is created via: `Signal.From<T>(Flow<T> flow)`.

Example:  
```csharp
var flow =
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;
var signal = Signal.From(flow);
```
### Sending Values Through the Flow
Once you have a signal, you can push values into the flow by calling: `Signal.Pulse(...)`.

For example, sending the value `42` into the flow:  
```csharp
 Signal.From(
        from anInt in Pulse.Start<int>()
        from trace in Pulse.Trace(anInt)
        select anInt)
    .Pulse(42);
```
### Capturing the Trace
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
## Pulsing a Flow: One Signal, One State

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
  
### From
`Signal.From(...)` is a simple factory method used to get hold of a `Signal<T>` instance
that wraps the passed in `Flow<T>`.  
```csharp
var flow =
    from anInt in Pulse.Start<int>()
    select anInt;
var signal = Signal.From(flow);
```
`Signal.From<T>(Func<T, Flow<Unit>>` is a useful overload that allows for inlining simple flows upon Signal creation.  
```csharp
var signal = Signal.From<int>(a => Pulse.Trace(a));
```
### Pulse
#### Pulsing One Value
`Signal.Pulse(...)` is the main way a flow can be instructed to do useful work.  
```csharp
var collector = TheCollector.Exhibits<int>();
Signal.From<int>(a => Pulse.Trace(a))
    .SetArtery(collector)
    .Pulse(42);
Assert.Single(collector.TheExhibit);
Assert.Equal(42, collector.TheExhibit[0]);
```
As the `Assert`'s demonstrate, this sends the int `42` into the flow.  
#### Pulsing Many Values
For ease of use, when dealing with `IEnumerable` return values from various sources, an overload exists: `Signal.Pulse(IEnumerable<T> inputs)`.   
```csharp
var collector = TheCollector.Exhibits<int>();
Signal.From<int>(a => Pulse.Trace(a))
    .SetArtery(collector)
    .Pulse([42, 43, 44]);
Assert.Equal(3, collector.TheExhibit.Count);
Assert.Equal(42, collector.TheExhibit[0]);
Assert.Equal(43, collector.TheExhibit[1]);
Assert.Equal(44, collector.TheExhibit[2]);
```
Same behaviour as the single-pulse example.  
#### Pulsing Nothing
Lastly, in some rare circumstances, a flow does not take any input. In `QuickPulse` *nothing* is represented by a `Unit` type.  
So in order to advance a flow of type `Flow<Unit>` you can use the `Signal.Pulse()` overload.  
```csharp
var flow =
    from input in Pulse.Start<Unit>()
    from _1 in Pulse.Prime(() => 42)
    from _2 in Pulse.Trace<int>(a => a)
    from _3 in Pulse.Manipulate<int>(a => a + 1)
    select input;
var collector = TheCollector.Exhibits<int>();
Signal.From(flow)
    .SetArtery(collector)
    .Pulse().Pulse().Pulse();
Assert.Equal(3, collector.TheExhibit.Count);
Assert.Equal(42, collector.TheExhibit[0]);
Assert.Equal(43, collector.TheExhibit[1]);
Assert.Equal(44, collector.TheExhibit[2]);
```
### The Heart
> Hunting for Flows.

  
The Heart is the typed Artery registry: it remembers where pulses can go, based on Artery type, and lets you target them deliberately.  
It is *not* an output by itself. 
  
#### The Main Artery
There is *always* exactly one Main Artery. It is the default outflow for a signal. Use `Signal.SetArtery(...)` to set it.  
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` emissions flow into it.    
```csharp
Signal.From<int>(a => Pulse.Trace(a))
    .SetArtery(holden) // <= 'holden' is now the Main Artery
    .Pulse(42);
```
`Signal.SetAndReturnArtery(...)` Similar, but returns the Artery you pass in (useful for quick wiring):  
```csharp
 Signal.From<int>(a => Pulse.Trace(a)).SetAndReturnArtery(TheString.Catcher());
```
Setting an Artery on a signal that already has one **replaces** the previous Artery.    
```csharp
var holden = TheString.Catcher();
var caulfield = TheString.Catcher();
Signal.From<int>(a => Pulse.Trace(a))
    .SetArtery(holden)
    .Pulse(42)
    .SetArtery(caulfield)
    .Pulse(43);
Assert.Equal("42", holden.Whispers());
Assert.Equal("43", caulfield.Whispers());
```
- Trying to set the Main Artery to null throws:  
    > The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?  
#### Grafting Extra Arteries
Apart from pulsing flows through the Main Artery, QuickPulse allows you to redirect flows to additional Arteries.  
There are various situations where this is useful.  
In the following section we will discuss how to set up one particular use case:
'Adding a diagnostic trace to an existing flow.' 

Suppose we have the following flow:   
```csharp
return
    from ch in Pulse.Start<char>()
    from depth in Pulse.Prime(() => -1)
    from _ in Pulse.TraceIf(depth >= 0, () => ch)
    from __ in Pulse.ManipulateIf<int>(ch == '{', x => x + 1)
    from ___ in Pulse.ManipulateIf<int>(ch == '}', x => x - 1)
    select ch;
```
This is a simple flow that returns the text between braces, even if there are other braces inside said text.  
**An Example**:  
```csharp
var holden = TheString.Catcher();
Signal.From(flow)
    .SetArtery(holden)
    .Pulse("{ a { b } c }");
```
Unfortunately the result of this is ` a { b } c }` and really, we want it to be ` a { b } c `.    

So let's try and find out what's going on.  

First we define a new typed Artery:  
```csharp
public class Diagnostic : Collector<string> { }
```
Then we *Graft* it onto the Heart through the `Signal.Graft(...)` method.  
```csharp
var holden = TheString.Catcher();
var diagnostic = new Diagnostic();
Signal.From(flow)
    .SetArtery(holden)
    .Graft(diagnostic)
    .Pulse("{ a { b } c }");
```
In this case, we could just Graft the `TheCollector<string>`, but creating a derived class expresses our intent much better.

Lastly we add a `Pulse.TraceTo<TArtery>(...)` to the flow:
  
```csharp
var flow = 
    from ch in Pulse.Start<char>()
    from depth in Pulse.Prime(() => -1)
    let enter = depth
    let emit = depth >= 0
    from _ in Pulse.TraceIf(emit, () => ch)
    from __ in Pulse.ManipulateIf<int>(ch == '{', x => x + 1)
    from ___ in Pulse.ManipulateIf<int>(ch == '}', x => x - 1)
    from exit in Pulse.Draw<int>()
    from diag in Pulse.TraceTo<Diagnostic>(
        $"char='{ch}', enter={enter}, emit={emit}, exit={exit}")
    select ch;
```
When executing this, the `Holden` Artery contains the same as before, but now we have the following in the `Diagnostic` Artery:  
```csharp
[
    "char='{', enter=-1, emit=False, exit=0",
    "char=' ', enter=0, emit=True, exit=0",
    "char='a', enter=0, emit=True, exit=0",
    "char=' ', enter=0, emit=True, exit=0",
    "char='{', enter=0, emit=True, exit=1",
    "char=' ', enter=1, emit=True, exit=1",
    "char='b', enter=1, emit=True, exit=1",
    "char=' ', enter=1, emit=True, exit=1",
    "char='}', enter=1, emit=True, exit=0",
    "char=' ', enter=0, emit=True, exit=0",
    "char='c', enter=0, emit=True, exit=0",
    "char=' ', enter=0, emit=True, exit=0",
    "char='}', enter=0, emit=True, exit=-1"
];
```
We can now use this information to correct the original flow  
#### GetArtery
`Signal.GetArtery<TArtery>(...)` can be used to retrieve the current `IArtery` set on the signal.  
  
```csharp
var holden =
    Signal.From<int>(a => Pulse.Trace(a))
        .SetArtery(TheString.Catcher())
        .Pulse(42)
        .GetArtery<Holden>();
Assert.Equal("42", holden.Whispers());
```
`Signal.GetArtery<TArtery>(...)` throws if trying to retrieve a concrete type of `IArtery` that the heart is unaware of.
      
## Memory And Manipulation
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
### Prime: one-time lazy initialization.
`Prime(() => T)` computes and stores a value **once per signal lifetime**.  
### Draw: read from memory.
`Draw<T>()` retrieves the current value from the signal's memory for type `T`.  
### Manipulate: controlled mutation of *primed* state.
`Manipulate<T>(Func<T,T>)` updates the current value of the gathered cell for type `T`.  
The return value of `Manipulate` is the **new value**, which can be used immediately in the flow.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 0)
    from i in Pulse.Manipulate<int>(x => x + 10) // <= update int cell
    from _2 in Pulse.Trace(i + input)            // <= use the new value
    select input;
var latch = TheLatch.Holds<int>();
Signal.From(flow).SetArtery(latch).Pulse(32);
Assert.Equal(42, latch.Q);
```
### Scoped: temporary overrides with automatic restore.
`Scoped<T>(enter, innerFlow)` runs `innerFlow` with a **temporary** value for the gathered cell of type `T`. On exit, the outer value is restored.  
Any `Manipulate<T>` inside the scope affects the **scoped** value and is discarded on exit.  
### Type Identity Matters
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
### Postfix Operators
Although the behaviour is logical once you think about it, it can feel a bit unintuitive,
but when using Postfix operators, beware that they return the *old* value.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.Prime(() => 0)
    from __ in Pulse.Manipulate<int>(a => a++) // <= int is still 0 in memory cell
    from now in Pulse.Trace<int>(a => a + input)
    select input;
var latch = TheLatch.Holds<int>();
Signal.From(flow).SetArtery(latch).Pulse(42);
Assert.Equal(42, latch.Q);
```
Use prefix form or pure expressions instead.  
* **Recommended:** `Pulse.Manipulate<int>(a => a + 1)`  
## Make It Flow
> Building larger behaviours from tiny flows.  

QuickPulse is about *composing* small, predictable `Flow<T>` building blocks. 
This chapter shows how to wire those flows together.  
### Then
`Then` runs `flow`, discards its value, and continues with `next` in the **same** state.
It's the flow-level equivalent of do this, *then* do that.  
```csharp
var dot = Pulse.Trace(".");
var space = Pulse.Trace(" ");
var flow =
    from input in Pulse.Start<int>()
    from _1 in dot.Then(dot).Then(dot).Then(space) // <=
    from _2 in Pulse.Trace(input)
    select input;
// Pulse 42 => results in '... 42'.
```
### ToFlow
Given this *sub* flow:  
```csharp
private static Flow<int> SubFlow()
{
    return
        from input in Pulse.Start<int>()
        from _ in Pulse.Trace(input + 1)
        select input;
}
```
`Pulse.ToFlow(...)` Executes a subflow over a value.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.ToFlow(SubFlow(), input)    // <=
    select input;
var latch = TheLatch.Holds<int>();
Signal.From(flow).SetArtery(latch).Pulse(41);
Assert.Equal(42, latch.Q);
```
An overload exist that allows for executing a subflow over a collection of values.  
```csharp
var flow =
    from input in Pulse.Start<List<int>>()
    from _ in Pulse.ToFlow(SubFlow(), input)    // <=
    select input;
var collector = TheCollector.Exhibits<int>();
var signal = Signal.From(flow).SetArtery(collector);
signal.Pulse([41, 41]);
Assert.Equal([42, 42], collector.TheExhibit);
```
Furthermore both the above methods can be used with a *Flow Factory Method*.  
Single value:  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.ToFlow(a => Pulse.Trace(a + 1), input)    // <=
    select input;
var latch = TheLatch.Holds<int>();
var signal = Signal.From(flow).SetArtery(latch);
signal.Pulse(41);
Assert.Equal(42, latch.Q);
```
Multiple values:  
```csharp
var flow =
    from input in Pulse.Start<List<int>>()
    from _ in Pulse.ToFlow(a => Pulse.Trace(a + 1), input)    // <=
    select input;
var collector = TheCollector.Exhibits<int>();
var signal = Signal.From(flow).SetArtery(collector);
signal.Pulse([41, 41]);
Assert.Equal([42, 42], collector.TheExhibit);
```
## Bending The River
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
### Using a Ternary Conditional Operator (*If/Then/Else*)
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
### When
```csharp
    from input in Pulse.Start<int>()
    from _ in Pulse.When(input % 2 == 0, Pulse.Trace("even"))
    select input;
```
##### Flow Factory Version
```csharp
    from input in Pulse.Start<int>()
    from _ in Pulse.When(input % 2 == 0, () => Pulse.Trace("even"))
    select input;
```
##### Using State
```csharp
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 2)
    from _2 in Pulse.When<int>(a => input % a == 0, Pulse.Trace("even"))
    select input;
```
##### Using State: Flow Factory Version
```csharp
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 2)
    from _2 in Pulse.When<int>(a => input % a == 0, () => Pulse.Trace("even"))
    select input;
```
> **Why the factory overloads?**  
> `When(..., () => flow)` and many other `Pulse` methods using the factory flow pattern defer creating the subflow/value until the condition is true, avoiding work and allocations on the false path.  
### TraceIf
```csharp
    from input in Pulse.Start<int>()
    from _ in Pulse.TraceIf(input % 2 == 0, () => "even")
    select input;
```
##### Using State
```csharp
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 2)
    from _2 in Pulse.TraceIf<int>(a => input % a == 0, () => "even")
    select input;
```
##### Using State, ... Twice
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
### ToFlowIf
All examples in this section use the following *sub* flow definition:  
```csharp
    from input in Pulse.Start<int>()
    from _ in Pulse.Trace(input.ToString())
    select input;
```
The `ToFlowIf` family conditionally executes a subflow.

Use it to embed optional or context-sensitive subflows declaratively.  
```csharp
    from input in Pulse.Start<int>()
    from _ in Pulse.ToFlowIf(input % 2 == 0, subFlow, () => input)
    select input;
```
Results in:  
```csharp
 ["2", "4"]
```
An overload exist that allows for executing a subflow over a collection of values.  
Given the following `Signal`:  
```csharp
Signal.From(flow)
    .SetArtery(TheCollector.Exhibits<string>())
    .Pulse([42, 43])
    .Pulse([1, 2, 3, 4, 5])
    .Pulse([42, 43, 44])
    .GetArtery<Collector<string>>()
    .TheExhibit;
```
And this `Flow`:  
```csharp
    from input in Pulse.Start<List<int>>()
    from _ in Pulse.ToFlowIf(input.Count == 5, subFlow, () => input)
    select input;
```
```csharp
 ["1", "2", "3", "4", "5"]
```
Both the above methods can be used with a *Flow Factory Method*.  
Single value:  
```csharp
    from input in Pulse.Start<int>()
    from _ in Pulse.ToFlowIf(input % 2 == 0, a => Pulse.Trace(a.ToString()), () => input)
    select input;
```
Multiple values:  
```csharp
    from input in Pulse.Start<List<int>>()
    from _ in Pulse.ToFlowIf(input.Count == 5, a => Pulse.Trace(((int)a).ToString()), () => input)
    select input;
```
##### Using State
```csharp
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 2)
    from _2 in Pulse.ToFlowIf<int, int>(a => input % a == 0, subFlow, () => input)
    select input;
```
##### Using State: Collection.
```csharp
    from input in Pulse.Start<List<int>>()
    from _1 in Pulse.Prime(() => 5)
    from _2 in Pulse.ToFlowIf<int, int>(a => input.Count == a, subFlow, () => input)
    select input;
```
##### Using State: Factory
```csharp
    from input in Pulse.Start<int>()
    from _1 in Pulse.Prime(() => 2)
    from _2 in Pulse.ToFlowIf<int, int>(a => input % a == 0, a => Pulse.Trace(a.ToString()), () => input)
    select input;
```
```csharp
    from input in Pulse.Start<List<int>>()
    from _1 in Pulse.Prime(() => 5)
    from _2 in Pulse.ToFlowIf<int, int>(a => input.Count == a, a => Pulse.Trace(a.ToString()), () => input)
    select input;
```
## Arteries Included
QuickPulse comes with a couple of built-in arteries:  
### The Shunt, a.k.a. `/dev/null`
The **Shunt** is the default artery installed in every new signal.  
It implements the Null Object pattern: an inert artery that silently absorbs all data.
Any call to `Absorb()` on a shunt simply vanishes, no storage, no side effects, no errors.
This ensures that flows without an explicitly attached artery still execute safely.  
### The Collector
The **Collector** is a simple artery that **gathers** every absorbed value into an internal collection.
It is primarily used in tests and diagnostics to verify what data a signal emits.
Each call to `Absorb()` appends the incoming objects to the exhibit list, preserving order.
Think of it as a **curator** for your flows, nothing escapes notice, everything is archived for later inspection.

Example:  
```csharp
var collector = TheCollector.Exhibits<string>();
Signal.From<string>(a => Pulse.Trace(a))
    .SetArtery(collector)
    .Pulse(["hello", "collector"]);
Assert.Equal("hello", collector.TheExhibit[0]);
Assert.Equal("collector", collector.TheExhibit[1]);
```
### The Latch
The **Latch** is a tiny, type-safe last-value latch. It simply remembers the most recent value absorbed and exposes it via `Q`.  
This is ideal for tests and probes where you only care about what came out last.

Example:  
```csharp
var latch = TheLatch.Holds<string>();
Signal.From<string>(a => Pulse.Trace(a))
    .SetArtery(latch)
    .Pulse(["hello", "latch"]);
Assert.Equal("latch", latch.Q);
```
### The Ledger
The **Ledger**` is a **persistent artery**, it records every absorbed value into a file.
Where `TheCollector` keeps its exhibits in memory, `TheLedger` writes them down for posterity.
It is ideal for tracing long-running flows or auditing emitted data across multiple runs.
Think of it as your **flow accountant**, keeping a faithful record of every transaction.  

Example:
  
```csharp
var filePath =
    Signal.From<string>(a => Pulse.Trace(a))
        .SetArtery(TheLedger.Records())
        .Pulse(["hello", "filesystem"])
        .GetArtery<Ledger>()
        .FilePath;
var lines = File.ReadAllLines(filePath);
Assert.Equal("hello", lines[0]);
Assert.Equal("filesystem", lines[1]);
```
When a filename is not explicitly provided, a unique file is automatically created in a .quickpulse directory
located at the solution root (i.e., the nearest parent directory containing a .sln file).  

The filename follows this pattern:
```bash
/solution/.quickpulse/quick-pulse-{unique-suffix}.log
```
This ensures that each run generates a distinct, traceable log file without overwriting previous logs.
  
You can, of course, pass in a custom filename.  
In that case, a `myfilename.log` file is created, still in the nearest parent directory that contains a `.sln` file.  

Example:  
```csharp
 TheLedger.Records("myfilename.log").FilePath;
```
Note that the `Ledger` will throw an exception if no `.sln` file can be found.  
The `TheLedger.Rewrites()` factory method does exactly what it says: it clears the file before logging.
This is an idiomatic way to log repeatedly to a file that should start out empty:  
### TheStringCatcher
This catcher quietly captures everything that flows through it, and returns it as a single string.  
It is especially useful in testing and example scenarios where the full trace output is needed as a value.

Use the static helper `TheString.Catcher()` to create a new catcher.  
You can get a hold of the string through the `.Whispers()` method.  
```csharp
var holden = TheString.Catcher();
Signal.From(
        from x in Pulse.Start<int>()
        from _ in Pulse.Trace("x = ")
        from __ in Pulse.Trace(42)
        select x)
    .SetArtery(holden)
    .Pulse(42);
Assert.Equal("x = 42", holden.Whispers());
```
You can also reset/clear the *caught* values using the `.Forgets()` method.  
## Addendum: No Where
> A.k.a.: Why There Is No `.Where(...)` in QuickPulse LINQ.  

In standard LINQ-to-objects, the `where` clause is lazily applied and safely filters values *before* any downstream computation happens. This works because `IEnumerable<T>` defers evaluation until iteration.

But **QuickPulse uses monadic LINQ over computation flows** (`Flow<T>`), not sequences. In monadic LINQ, the C# compiler desugars `where` **after** any preceding `let`, `from`, or `select` clauses — and **evaluates them eagerly**.

This means:

```csharp
from x in Flow<T>
where x != null
let y = x.SomeProperty // NRE: still evaluated even if x is null!
```

The `let` runs *before* the `where`, causing runtime exceptions — even though it looks safe.
  
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
  
### And This Matters Because ... ?

Avoiding `.Where(...)` keeps evaluation order predictable and prevents accidental crashes in:

* Diagnostic flows
* Shrinking logic
* Custom combinators and trace sequences

It's a minor trade-off in exchange for greater composability and correctness.
  
## Why QuickPulse Exists
> A.k.a. A deep dark forest, a looking glass, and a trail of dead generators.

A little while back I was writing a test for a method that took some JSON as input.
I got my fuzzers out and went to work. And then... my fuzzers gave up.

So I added the following to **QuickFuzzr**:
```csharp
    var generator =
        from _ in Fuzz.For<Tree>().Depth(2, 5)
        from __ in Fuzz.For<Tree>().GenerateAsOneOf(typeof(Branch), typeof(Leaf))
        from ___ in Fuzz.For<Tree>().TreeLeaf<Leaf>()
        from tree in Fuzz.One<Tree>().Inspect()
        select tree;
```
Which can generate output like this:
```
    └── Node
        ├── Leaf(60)
        └── Node
            ├── Node
            │   ├── Node
            │   │   ├── Leaf(6)
            │   │   └── Node
            │   │       ├── Leaf(30)
            │   │       └── Leaf(21)
            │   └── Leaf(62)
            └── Leaf(97)
```
Neat. But this story isn't about the output, it's about the journey.  
Implementing this wasn't trivial. And I was, let's say, a muppet, more than once along the way.

Writing a unit test for a fixed depth like `(min:1, max:1)` or `(min:2, max:2)`? Not a problem.  
But when you're fuzzing with a range like `(min:2, max:5).` Yeah, ... good luck.

Debugging this kind of behaviour was as much fun as writing an F# compiler in JavaScript.  
So I wrote a few diagnostic helpers: visualizers, inspectors, and composable tools
that could take a generated value and help me see why things were behaving oddly.

Eventually, I nailed the last bug and got tree generation working fine.

Then I looked at this little helper I'd written for combining stuff and thought: **"Now *that's* a nice-looking rabbit hole."**

One week and exactly nine combinators later, I had this surprisingly useful, lightweight little library.
  
