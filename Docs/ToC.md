# QuickPulse Documentation

- [A Quick Pulse](AQuickPulse.md)
- [Make it Flow](MakeItFlow.md)
- [One Signal, One State](OneSignalOneState.md)
- [Flow Extensions](FlowExtensions.md)
- [Arteries Included](ArteriesIncluded.md)
- [Examples](Examples.md)
- [Addendum: No Where](NoWhere.md)

# QuickPulse

> **LINQ with a heartbeat.**
>
> A tiny library for building stateful, inspectable, composable flows.

## Getting Started

QuickPulse lets you write LINQ expressions that react to values, accumulate state, and emit traces along the way.

```csharp
Signal.From(
        from input in Pulse.Start<string>()
        from isFirst in Pulse.Gather(true)
        let capitalized = char.ToUpper(input[0]) + input[1..]
        let evenLength = input.Length % 2 == 0
        from _1 in Pulse.TraceIf(isFirst.Value, () => capitalized)
        from _2 in Pulse.TraceIf(!isFirst.Value, () => $" {input}")
        from _3 in Pulse.TraceIf(evenLength, () => ", a looking glass")
        from _ in Pulse.Effect(() => isFirst.Value = false)
        select input)
    .SetArtery(TheString.Catcher())
    .Pulse("a deep dark forest")
    .Pulse("and a trail of dead generators.")
    .GetArtery<Holden>()
    .Whispers();

// => "A deep dark forest, a looking glass and a trail of dead generators."
```

## Documentation

QuickPulse is fully documented, with real, executable examples for each combinator and concept.

You can explore it here:

* **[Table of Contents](./Docs/TOC.md)**

Or, ... see [Building a Flow](./Docs/BuildingAFlow.md) for a hands-on quickstart.

## Highlights

* Composable `Flow<T>` pipelines using LINQ.
* Stateful `Signal<T>` execution.
* Built-in tracing via `IArtery`.
* Conditional logic (`TraceIf`, `EffectIf`, `When`).
* Scoped state and reversible manipulations.
* Integration with file logging and string capture.

## Why QuickPulse Exists?

Do you know how you sometimes leave your house, maybe to get some cigarettes, and start thinking about something?
Your brain takes over.
You walk straight past the shop, and the legs just keep going.
An hour later, you look up, and you're in the next village wondering how you got there.

No? Just me?

Well, okay.

It happens in code too, ... quite a lot.
This library is the result of one of those walks through a dark forest.
And yes, it did *literally* involve Trees.


## Installation

QuickPulse is available on NuGet:

```bash
Install-Package QuickPulse
```

Or via the .NET CLI:

```bash
dotnet add package QuickPulse
```

# A Quick Look
To explain how QuickPulse works (not least to myself), let's build up a flow step by step.
## The Minimal Flow

```csharp
from anInt in Pulse.Start<int>()
select anInt;
```

The type generic in `Pulse.Start<T>` defines the **input type** to the flow.
**Note:** It is required to select the result of `Pulse.Start(...)` at the end of the LINQ chain for the flow to be considered well-formed.
## Doing Something with the Input
Let's trace the values as they pass through:

```csharp
from anInt in Pulse.Start<int>()
from trace in Pulse.Trace(anInt)
select anInt;
```

## Executing a Flow
To execute a flow, we need a `Signal<T>`, which is created via:

```csharp
Signal.From<T>(Flow<T> flow)
```

Example:

```csharp
var flow =
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;

var signal = Signal.From(flow);
```

## Sending Values Through the Flow
Once you have a signal, you can push values into the flow by calling:

```csharp
Signal.Pulse(params T[] input)
```

Example:

```csharp
var flow =
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;

var signal = Signal.From(flow);
signal.Pulse(42);
```

This sends the value `42` into the flow.

## Capturing the Trace
To observe what flows through, we can add an `IArtery` by using `SetArtery` directly on the signal.

```csharp
[Fact]
public void Adding_an_artery()
{
    var flow =
        from anInt in Pulse.Start<int>()
        from trace in Pulse.Trace(anInt)
        select anInt;

    var collector = new TheCollector<int>();

    Signal.From(flow)
        .SetArtery(collector)
        .Pulse(42, 43, 44);

    Assert.Equal(3, collector.TheExhibit.Count);
    Assert.Equal(42, collector.TheExhibit[0]);
    Assert.Equal(43, collector.TheExhibit[1]);
    Assert.Equal(44, collector.TheExhibit[2]);
}
```


# Make it Flow
**Number One's Cheat Sheet:**

| Combinator                                    | Role / Purpose                                                                |
| --------------------------------------------- | ----------------------------------------------------------------------------- |
| [**Start<T>()**](./MakeItFlowStart.md)        | Starts a new flow. Defines the input type.                                    |
| [**Trace(...)**](./MakeItFlowTrace.md)        | Emits trace data unconditionally to the current artery.                       |
| [**TraceIf(...)**](./MakeItFlowTrace.md)      | Emits trace data conditionally, based on a boolean flag.                      |
| [**FirstOf(...)**](./MakeItFlowFirstOf.md)    | Executes the first flow where its condition is `true`, skips the rest.        |
| [**Effect(...)**](./MakeItFlowEffect.md)      | Performs a side-effect (logging, mutation, etc.) without yielding a value.    |
| [**EffectIf(...)**](./MakeItFlowEffect.md)    | Performs a side-effect conditionally.                                         |
| [**Gather<T>(...)**](./MakeItFlowGather.md)   | Captures a mutable box into flow memory (first write wins).                   |
| [**Scoped<T>(...)**](./MakeItFlowScoped.md)   | Temporarily mutates gathered state during a subflow, then restores it.        |
| [**ToFlow(...)**](./MakeItFlowToFlow.md)      | Invokes a subflow over a value or collection.                                 |
| [**ToFlowIf(...)**](./MakeItFlowToFlow.md)    | Invokes a subflow conditionally, using a supplier for the input.              |
| [**When(...)**](./MakeItFlowWhen.md)          | Executes the given flow only if the condition is true, without input.         |
| [**NoOp()**](./MakeItFlowNoOp.md)             | Applies a do-nothing operation (for conditional branches or comments).        |

# Effect

**`Pulse.Effect(...)`** Executes a side-effect without yielding a value.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1)
from eff in Pulse.Effect(() => box.Value++) // <=
select anInt;
```
**Warning:** `Effect` performs side-effects.
It is eager, observable, and runs even if you ignore the result.
Use when you mean it.

# When

**`Pulse.When(...)`** Executes a subflow conditionally.

A flow that does not take an input like `var someMessage = Pulse.Trace("Some Message")` can be defined as a sub flow,
and executed by simple including it in the Linq chain: `from _ in someMessage`.

If we want to flow, based on a predicate, we could do: `from _ in predicate ? someMessage : Pulse.NoOp()`.

Which is fine but with `Pulse.When(...)` we can do better.

**Example:**
```csharp
var dotDotDot = Pulse.Trace("...");
var flow =
    from anInt in Pulse.Start<int>()
    from _ in Pulse.When(anInt == 42, dotDotDot) // <=
    select anInt;
var collector = new TheCollector<string>();
Signal.From(flow).SetArtery(collector)
    .Pulse(6)
    .Pulse(42);
Assert.Equal(["..."], collector.TheExhibit);
```

# Trace

**`Pulse.Trace(...)`** emits trace data unconditionally to the current artery.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.Trace(anInt) // <=
select anInt;
```

# ToFlow

**`Pulse.ToFlow(...)`** Executes a subflow over a value or collection.

**Example:**
```csharp
var subFlow =
    from anInt in Pulse.Start<int>()
    from _ in Pulse.Trace(anInt)
    select anInt;
var flow =
    from box in Pulse.Start<Box<int>>()
    from _ in Pulse.ToFlow(subFlow, box.Value) // <=
    select box;
```

# Start
Every flow definition needs to start with a call to `Pulse.Start()`.
This strongly types the values that the flow can receive.
In addition, the result of the call needs to be used in the select part of the LINQ expression.
This strongly types the flow itself.

**Example:**
```csharp
from anInt in Pulse.Start<int>() // <=
select anInt;
```

# Scoped
**`Pulse.Scoped<T>(...)`** temporarily alters gathered state of type T, runs an inner flow,
and reverts the state after.
**Example:**
```csharp
var collector = new TheCollector<int>();
var innerFlow =
    from anInt in Pulse.Start<int>()
    from scopedBox in Pulse.Gather<int>()
    from _ in Pulse.Trace(anInt + scopedBox.Value)
    select anInt;
var flow =
    from anInt in Pulse.Start<int>()
    from box in Pulse.Gather(0)
    from _ in Pulse.Trace(anInt + box.Value)
    from scopeInt in Pulse.Scoped<int>(
        a => a + 1,
        Pulse.ToFlow(innerFlow, anInt))
    from __ in Pulse.Trace(anInt + box.Value)
    select anInt;
var signal = Signal.From(flow).SetArtery(collector);
signal.Pulse(42);
Assert.Equal([42, 43, 42], collector.TheExhibit);
```
# NoOp

**`Pulse.NoOp(...)`** A do-nothing operation (useful for conditional branches). 

**Example:**
```csharp
from anInt in Pulse.Start<int>()
    from _ in Pulse
        .NoOp(/* --- Also useful for Comments --- */)
    select anInt;
```

# Gather
**`Pulse.Gather(...)`** Binds a mutable box into flow memory (first write wins).

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1) // <=
select anInt;
```
# FirstOf

**`Pulse.FirstOf(...)`** runs the first flow in a sequence of (condition, flow) pairs where the condition evaluates to true.

**Example:**
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.TraceFirstOf(
        (() => input == 42, () => Pulse.Trace("answer")),
        (() => input == 666, () => Pulse.Trace("beëlzebub")),
        (() => input == 42 || input == 666, () => Pulse.Trace("never")))
    select input;
```


**`Pulse.TraceIf(...)`** emits trace data conditionally, based on a boolean flag.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.TraceIf(anInt != 42, () => anInt) // <=
select anInt;
```


**`Pulse.ToFlowIf(...)`** Executes a subflow over a value or collection, conditionally.

**Example:**
```csharp
var subFlow =
    from anInt in Pulse.Start<int>()
    from _ in Pulse.Trace(anInt)
    select anInt;
var flow =
    from box in Pulse.Start<Box<int>>()
    from _ in Pulse.ToFlowIf(box.Value != 42, subFlow, () => box.Value) // <=
    select box;
```

**`Pulse.Gather<T>()`** used without an argument, serves as a 'getter' of a previously gathered value.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1)
from val in Pulse.Gather<int>() // <=
from _ in Pulse.Trace(anInt + val.Value)
select anInt;
```
**`Pulse.Gather<T>()`** throws if no value of the requested type is available.

**`Pulse.EffectIf(...)`** Same as above, but conditional. 

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from seen42 in Pulse.Gather(false)
from eff in Pulse.EffectIf(anInt == 42, () => seen42.Value = true) // <=
select anInt;
```

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
# Flow Extensions
Not a big fan of extensions on LINQ enabled combinators, but there *is* one which is just to useful to pass up on.
## Then
**`.Then(...)`** is just syntactic sugar for `.SelectMany(...)`.

Suppose we have: 
```csharp
var dot = Pulse.Trace(".");
var space = Pulse.Trace(" ");
```
We can compose this like so:
```csharp
var threeDotsAndSpace =
    from d1 in dot
    from d2 in dot
    from d3 in dot
    from s in space
    select Unit.Instance;
```
Most of you would probably prefer: 
```csharp
var threeDotsAndSpace = dot.SelectMany(_ => dot).SelectMany(_ => dot).SelectMany(_ => space);
```
Now with `.Then(...)` you can do:
```csharp
var threeDotsAndSpace = dot.Then(dot).Then(dot).Then(space);
```

# Arteries Included
QuickPulse comes with three built-in arteries:
## TheCollector
This is the artery used throughout the documentation examples, and it's especially useful in testing scenarios.

Example:
```csharp
var collector = new TheCollector<string>();
Signal.Tracing<string>()
    .SetArtery(collector)
    .Pulse("hello", "collector");
Assert.Equal("hello", collector.TheExhibit[0]);
Assert.Equal("collector", collector.TheExhibit[1]);
```

## WriteDataToFile
This artery is included because writing trace output to a file is one of the most common use cases.
Example:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile())
    .Pulse("hello", "collector");
```
The file will contain:
```
hello
collector
```

When a filename is not explicitly provided, a unique file is automatically created in a .quickpulse directory
located at the solution root (i.e., the nearest parent directory containing a .sln file).  

The filename follows this pattern:
```bash
/solution/.quickpulse/quick-pulse-{unique-suffix}.log
```
This ensures that each run generates a distinct, traceable log file without overwriting previous logs.

You can, of course, pass in a custom filename.
Example:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile("myfilename.log"))
    .Pulse("hello", "collector");
```
In that case, a `myfilename.log` file is created, still in the nearest parent directory that contains a `.sln` file.

Note that the `WriteDataToFile` constructor will throw an exception if no `.sln` file can be found.
To avoid solution root detection altogether, use the following factory method:
```csharp
Signal.Tracing<string>()
    .SetArtery(WriteDataToFile.UsingHardCodedPath("hard.txt"))
    .Pulse("hello", "collector");
```

`WriteDataToFile` appends all entries to the file; each pulse adds new lines to the end.

The `ClearFile` method does exactly what it says: it clears the file before logging.
This is an idiomatic way to log repeatedly to a file that should start out empty:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile().ClearFile())
    .Pulse("hello", "collector");
```

### Sugaring
These are simple aliases that make common cases easier to read:
- `WriteData.ToFile(...)` is shorthand for `new WriteDataToFile(...)`
- `WriteData.ToNewFile(...)` is shorthand for `new WriteDataToFile(...).ClearFile()`
## TheStringCatcher
This catcher quietly captures everything that flows through it, and returns it as a single string.  
It is especially useful in testing and example scenarios where the full trace output is needed as a value.

Use the static helper `TheString.Catcher()` to create a new catcher:
```csharp
var holden = TheString.Catcher();
```
You can get a hold of the string through the `.Whispers()` method.
```csharp
var holden = TheString.Catcher();
Signal.From(
        from x in Pulse.Start<int>()
        from _ in Pulse.Trace($"x = {x}")
        select x)
    .SetArtery(holden)
    .Pulse(42);
Assert.Equal("x = 42", holden.Whispers());
```
Great for assertions or string comparisons in tests.
# Some Examples
## Log Filtering

**Given:**
```csharp
public record DiagnosticInfo(string[] Tags, string Message, int PhaseLevel);
```
We can filter this by tags and indent output based on PhaseLevel like so: 
```csharp
public static Signal<DiagnosticInfo> FilterOnTags(IArtery artery, params string[] filter)
{
    var flow =
        from _ in Pulse.Using(artery)
        from diagnosis in Pulse.Start<DiagnosticInfo>()
        let needsLogging = diagnosis.Tags.Any(a => filter.Contains(a))
        let indent = new string(' ', diagnosis.PhaseLevel * 4)
        from log in Pulse.TraceIf(needsLogging, $"{indent}{diagnosis.Tags.First()}:{diagnosis.Message}")
        select diagnosis;
    return Signal.From(flow);
}
```

## Rendering this Document

```csharp
public static Flow<DocAttribute> RenderMarkdown =
    from doc in Pulse.Start<DocAttribute>()
    let headingLevel = doc.Order.Split('-').Length
    from rcaption in Pulse
        .NoOp(/* ---------------- Render Caption  ---------------- */ )
    let caption = doc.Caption
    let hasCaption = !string.IsNullOrEmpty(doc.Caption)
    let headingMarker = new string('#', headingLevel)
    let captionLine = $"{headingMarker} {caption}"
    from _t2 in Pulse.TraceIf(hasCaption, () => captionLine)
    from rcontent in Pulse
        .NoOp(/* ---------------- Render content  ---------------- */ )
    let content = doc.Content
    let hasContent = !string.IsNullOrEmpty(content)
    from _t3 in Pulse.TraceIf(hasContent, () => content)
    from end in Pulse
        .NoOp(/* ---------------- End of content  ---------------- */ )
    select doc;
```

# Why QuickPulse Exists
*A.k.a. A deep dark forest, a looking glass, and a trail of dead generators.*

A little while back I was writing a test for a method that took some JSON as input.
I got my fuzzers out and went to work. And then... my fuzzers gave up.

So I added the following to **QuickMGenerate**:
```csharp
    var generator =
        from _ in MGen.For<Tree>().Depth(2, 5)
        from __ in MGen.For<Tree>().GenerateAsOneOf(typeof(Branch), typeof(Leaf))
        from ___ in MGen.For<Tree>().TreeLeaf<Leaf>()
        from tree in MGen.One<Tree>().Inspect()
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

Debugging this kind of behavior was as much fun as writing an F# compiler in JavaScript.  
So I wrote a few diagnostic helpers: visualizers, inspectors, and composable tools
that could take a generated value and help me see why things were behaving oddly.

Eventually, I nailed the last bug and got tree generation working fine.

Then I looked at this little helper I'd written for combining stuff and thought: **"Now *that's* a nice-looking rabbit hole."**

One week and exactly nine combinators later, I had this surprisingly useful, lightweight little library.

# Addendum: No Where
## A.k.a.: Why There Is No `.Where(...)` in QuickPulse LINQ
In standard LINQ-to-objects, the `where` clause is lazily applied and safely filters values *before* any downstream computation happens. This works because `IEnumerable<T>` defers evaluation until iteration.

But **QuickPulse uses monadic LINQ over computation flows** (`Flow<T>`), not sequences. In monadic LINQ, the C# compiler desugars `where` **after** any preceding `let`, `from`, or `select` clauses — and **evaluates them eagerly**.

This means:

```csharp
from x in Flow<T>
where x != null
let y = x.SomeProperty // NRE: still evaluated even if x is null!
```

The `let` runs *before* the `where`, causing runtime exceptions — even though it looks safe.

## Instead of `where`, use:

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

## And This Matters Because ... ?

Avoiding `.Where(...)` keeps evaluation order predictable and prevents accidental crashes in:

* Diagnostic flows
* Shrinking logic
* Custom combinators and trace sequences

It's a minor trade-off in exchange for greater composability and correctness.

