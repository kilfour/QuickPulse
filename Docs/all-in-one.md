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
var collector = TheCollector.Exhibits<int>();
Signal.From(
        from anInt in Pulse.Start<int>()
        from trace in Pulse.Trace(anInt)
        select anInt)
    .SetArtery(collector)
    .Pulse([42, 43, 44]);
// TheCollector.Exhibit now holds => [42, 43, 44]."
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
`Signal.From<T>(Func<T, Flow<Flow>>` is a useful overload that allows for inlining simple flows upon Signal creation.  
```csharp
var signal = Signal.From<int>(a => Pulse.Trace(a));
```
### Pulse
#### Pulsing One Value
`Signal.Pulse(...)` is the main way a flow can be instructed to do useful work.  
```csharp
Signal.From<int>(a => Pulse.Trace(a))
    .Pulse(42)
    .Pulse(43)
    .Pulse(44);
```
This sends the int's `42`, `43` and `44` into the flow.  
#### Pulsing Many Values
For ease of use, when dealing with `IEnumerable` return values from various sources, an overload exists: `Signal.Pulse(IEnumerable<T> inputs)`.   
```csharp
Signal.From<int>(a => Pulse.Trace(a))
    .Pulse([42, 43, 44]);
```
Same behaviour as the single pulse example.  
#### Pulsing Nothing
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
### Flatline
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
## Memory And Manipulation
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
### Prime: one-time lazy initialization.
`Prime(() => T)` computes and stores a value **once per signal lifetime**.  
### Draw: read from memory.
`Draw<T>()` retrieves the current value from the signal's memory for type `T`.  
The `Draw<TBox, T>(Func<TBox, T> func)` is just a bit of sugar to enable accessing nested values.  
### State aware overloads.
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
### Manipulate: controlled mutation of *primed* state.
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
### Scoped: temporary overrides with automatic restore.
`Scoped<T>(enter, innerFlow)` runs `innerFlow` with a **temporary** value for the *gathered cell* of type `T`. On exit, the outer value is restored.  
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
 from _ in Pulse.Start<Flow>()
       from _1 in Pulse.Prime(() => new Int1(1))
       from _2 in Pulse.Prime(() => new Int2(2))
       from _3 in Pulse.Trace(_1.Number + _2.Number)
       select Flow.Continue;
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
Signal.From(flow).Pulse(41);
// Result => 41. Not 42!
```
Use prefix form or pure expressions instead.  
* **Recommended:** `Pulse.Manipulate<int>(a => a + 1)`  
## Circulation
> Make it flow, number one.  

While it is entirely possible, and sometimes weirdly intellectually satisfying,
to write an entire QuickPulse Flow as one big LINQ expression,
it would be silly to ignore one of the main strengths of the LINQy approach: Composability.

QuickPulse provides two main ways to achieve this.  
### Then
The `Then` combinator joins two flows sequentially while sharing the same internal state.
It's the flow-level equivalent of saying *do this, then that*.  
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
If `Then` is about sequence, `ToFlow` is about delegation. It executes another flow *as part* of the current one.  
```csharp
var subflow =
    from input in Pulse.Start<int>()
    from _ in Pulse.Trace<int>(a => input + a)
    select input;
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.Prime(() => 1)
    from __ in Pulse.ToFlow(subflow, input)    // <=
    select input;
// Pulse 41 => results in 42.
```
This lets you reuse a named or shared flow inside another.
The subflow inherits the same signal state, so memory cells and arteries are visible across layers.  
`ToFlow` can also iterate through collections:  
```csharp
var subflow =
    from input in Pulse.Start<int>()
    from result in Pulse.Manipulate<int>(a => a + input)
    select input;
var flow =
    from input in Pulse.Start<List<int>>()
    from _1 in Pulse.Prime(() => 0)
    from _2 in Pulse.ToFlow(subflow, input)
    from _3 in Pulse.Trace<int>(a => $"Sum = {a}")
    select input;
// Pulse [1, 2, 3] => results in "Sum = 6".
```
This version of `ToFlow` is the declarative way to write what would otherwise be a `loop`, `foreach`, `for`, etcetera.  
### Query Syntax vs Method Syntax
> Maybe now is the time to talk about Kevin.  

Another feature of LINQ is the two syntactically different but computationally equal styles of expression.  
In general the query syntax is more declarative (*what* you want to do),
while the method syntax can be more practical (*how* it actually executes).  

QuickPulse offers two similar dialects. The examples above are written in what could be called QuickPulse **query syntax**.  
Here are the same examples rewritten using **method syntax**:  
```csharp
var dot = Pulse.Trace(".");
var space = Pulse.Trace(" ");
Pulse.Start<int>(a =>
    dot.Then(dot).Then(dot).Then(space).Then(Pulse.Trace(a)));
```
```csharp
Pulse.Start<int>(a =>
    Pulse.Prime(() => 1)
        .Then(Pulse.ToFlow(b => Pulse.Trace<int>(c => b + c), a)));
```
```csharp
Pulse.Start<List<int>>(numbers =>
    Pulse.Prime(() => 0)
        .Then(Pulse.ToFlow(a => Pulse.Manipulate<int>(b => a + b).Dissipate(), numbers))
        .Then(Pulse.Trace<int>(a => $"Sum = {a}")));
```
Ultimately, the choice between query syntax and method syntax comes down to readability and personal preference.
Query syntax often provides a more declarative, linear flow that clearly expresses the sequence of operations,
while method syntax can offer a more functional, compositional style that some developers find more natural.  

**Note:** The .`Dissipate()` extension method runs the targeted flow and discards its output, returning a `Flow<Flow>`.  
It's often used in method syntax to glue flows together seamlessly.  
## Capillaries and Arterioles
> A.k.a. Pulse Regulation. Branching and conditional control in QuickPulse.
 

So far we've mostly seen flows that travel forever on.
Useful for things like declarative composition,
but where would we be without the ability to branch off an Artery into an Arteriole or even a Capillary.

QuickPulse provides the following ways to control the *direction and branching* of a flow.  
### Using a Ternary Conditional Operator (*If/Then/Else*)
```csharp
var flow =
    from input in Pulse.Start<int>()
    let conditional =
        input % 2 == 0
        ? Pulse.Trace("even")
        : Pulse.Trace("uneven")
    from _ in conditional
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["uneven", "even", "uneven", "even", "uneven"].
```
Prefer `Pulse.NoOp()` when you want an if/then without an else-branch:  
```csharp
var flow =
    from input in Pulse.Start<int>()
    let conditional =
        input % 2 == 0
        ? Pulse.Trace("even")
        : Pulse.NoOp()
    from _ in conditional
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
```
*Note:* While the ternary operator works, QuickPulse provides more idiomatic ways to deal with conditional statemens, which we will look at below.  
### When
`Pulse.When` is the declarative equivalent of the ternary operator combined with `.NoOp()`.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.When(input % 2 == 0, Pulse.Trace("even"))
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
```
### The `Pulse.{SomeMethod}If()` Variants
In a similar vein to the state aware utility overloads,
most `Pulse` methods have an `If` variant that allows for conditional execution.  

  
**Examples:**  
  
*Conditional tracing:*  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.TraceIf(input % 2 == 0, () => "even")
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
```
*Branching a flow:*  
```csharp
var even = Pulse.Start<int>(_ => Pulse.Trace("even"));
var three = Pulse.Start<int>(_ => Pulse.Trace("three"));
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.ToFlowIf(input % 2 == 0, even, () => input)
    from __ in Pulse.ToFlowIf(input == 3, three, () => input)
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "three", "even"].
```
*Counting even numbers using `ManipulateIf()`:*  
```csharp
var even = Pulse.Start<int>(_ => Pulse.Trace("even"));
var three = Pulse.Start<int>(_ => Pulse.Trace("three"));
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.Prime(() => 0)
    from __ in Pulse.ManipulateIf<int>(input % 2 == 0, a => a + 1)
    from ___ in Pulse.Trace<int>(a => $"{input}: {a}")
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["1: 0", "2: 1", "3: 1", "4: 2", "5: 2"].
```
### FirstOf
Pulse.FirstOf(...) lets you chain multiple conditional flows and automatically
runs the first one whose condition evaluates to true.
It's like a compact, declarative if / else if / else ladder for flows.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.FirstOf(
        (() => input % 2 == 0, () => Pulse.Trace("even")),
        (() => input == 3, () => Pulse.Trace("three")))
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "three", "even"].
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
    .Pulse("hello")
    .Pulse("collector");
// collector.TheExhibit now equals ["hello", "collector"]
```
### The Latch
The **Latch** is a tiny, type-safe last-value latch. It simply remembers the most recent value absorbed and exposes it via `Q`.  
This is ideal for tests and probes where you only care about what came out last.

Example:  
```csharp
var latch = TheLatch.Holds<string>();
Signal.From<string>(a => Pulse.Trace(a))
    .SetArtery(latch)
    .Pulse("hello")
    .Pulse("latch");
// latch.Q now equals "latch"
```
### The Ledger
The `**Ledger**` is a **persistent artery**, it records every absorbed value into a file.
Where `TheCollector` keeps its exhibits in memory, `TheLedger` writes them down for posterity.
It is ideal for tracing long-running flows or auditing emitted data across multiple runs.
Think of it as your **flow accountant**, keeping a faithful record of every transaction.  

Example:
  
```csharp
var ledger = TheLedger.Records();
Signal.From<string>(a => Pulse.Trace(a))
    .SetArtery(ledger)
    .Pulse("hello")
    .Pulse("filesystem");
// File.ReadAllLines(...) now equals ["hello", "filesystem"]
```
When a filename is not explicitly provided, a unique file is automatically created in a .quickpulse directory
located at the nearest directory containing a .sln file (the solution root).  

The filename follows this pattern:
```bash
/solution/.quickpulse/quick-pulse-{unique-suffix}.log
```
This ensures that each run generates a distinct, traceable log file without overwriting previous logs.
  
You can, of course, pass in a custom filename.  
In that case, a `myfilename.log` file is created, still in the nearest parent directory that contains a `.sln` file.  

Example:  
```csharp
TheLedger.Records("myfilename.log");
```
Note that the `Ledger` will throw an exception if no `.sln` file can be found.  
The `TheLedger.Rewrites()` factory method does exactly what it says: it clears the file before logging.
This is an idiomatic way to log repeatedly to a file that should start out empty:  
### The String Catcher
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
var result = holden.Whispers(); // <=
// result now equals "x = 42"
```
You can also reset/clear the *caught* values using the `.Forgets()` method.  
## The Heart
> Hunting for Flows.

  
The Heart is the typed Artery registry: it remembers where pulses can go, based on Artery type, and lets you target them deliberately.  
It is *not* an output by itself. 
  
### The Main Artery
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
// holden.Whispers()    => "42"
// caulfield.Whispers() => "43"
```
- Trying to set the Main Artery to null throws:  
    > The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?  
### Grafting Extra Arteries
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
### GetArtery
`Signal.GetArtery<TArtery>(...)` can be used to retrieve the current `IArtery` set on the signal.  
  
```csharp
var holden =
    Signal.From<int>(a => Pulse.Trace(a))
        .SetArtery(TheString.Catcher())
        .Pulse(42)
        .GetArtery<Holden>(); // <=
// holden.Whispers() => "42"
```
`Signal.GetArtery<TArtery>(...)` throws if trying to retrieve a concrete type of `IArtery` that the heart is unaware of.
      
## Addendum: No Where
> A.k.a.: Why There Is No `.Where(...)` in QuickPulse LINQ.  

In standard LINQ-to-objects, the `where` clause is lazily applied and safely filters values *before* any downstream computation happens. This works because `IEnumerable<T>` defers evaluation until iteration.

But **QuickPulse uses monadic LINQ over computation flows (`Flow<T>`), not sequences**. In monadic LINQ, the C# compiler desugars `where` **after** any preceding `let`, `from`, or `select` clauses and **evaluates them eagerly**.

This means:

```csharp
from x in Flow<T>
where x != null
let y = x.SomeProperty // NRE: still evaluated even if x is null!
```

The `let` runs *before* the `where`, causing runtime exceptions even though it looks safe.
  
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
  
