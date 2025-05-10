# QuickPulse
Do you know how you sometimes leave your house—maybe to get some cigarettes—and start thinking about something?
Your brain takes over.
You walk straight past the shop, and the legs just keep going.
An hour later, you look up, and you're in the next village wondering how you got there.

No? Just me?

Well, okay.

It happens in code too, ... quite a lot.
This library is the result of one of those walks through a dark forest.
And yes, it did *literally* involve Trees.

# Building a Flow
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
To observe what flows through, we can add an `IArtery`.
There are a few ways to do this, here's one using `SetArtery` directly on the signal.

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



## The Life and Times of a Single Pulse
```                   
                     +-----------------------------+
Input via            |     Signal<T> instance      |
Signal.Pulse(x) ---> |  (wraps Flow<T> + state)    |
                     +-------------┬---------------+
                                   │
                                   ▼
                      +------------------------+
                      |    Flow<T> via LINQ    |
                      | (Start → Gather → ...) |
                      +------------------------+
                                   │
                  +----------------+----------------+
                  |                |                |
                  ▼                ▼                ▼
            +----------+     +-----------+     +-----------+
            | Gather() |     | Trace()   |     | ToFlow()  |
            | (state)  |     | (emit)    |     | (subflow) |
            +----------+     +-----------+     +-----------+
                                   │
                                   ▼
                        +------------------+
                        | Artery (optional) |
                        | Receives traces   |
                        +------------------+
```

# How To Pulse
**Cheat Sheet:**

| Combinator         | Role / Purpose                                                                |
| ------------------ | ----------------------------------------------------------------------------- |
| **Start<T>()**     | Starts a new flow. Defines the input type.                                    |
| **Using(...)**     | Applies an `IArtery` to the flow context, enables tracing.                   |
| **Trace(...)**     | Emits trace data unconditionally to the current artery.                       |
| **TraceIf(...)**   | Emits trace data conditionally, based on a boolean flag.                      |
| **Effect(...)**    | Performs a side-effect (logging, mutation, etc.) without yielding a value.    |
| **EffectIf(...)**  | Performs a side-effect conditionally.                                         |
| **Gather<T>(...)** | Captures a mutable box into flow memory (first write wins).                   |
| **ToFlow(...)**    | Invokes a subflow over a value or collection.                                 |
| **ToFlowIf(...)**  | Invokes a subflow conditionally, using a supplier for the input.              |
| **NoOp()**         | Applies a do-nothing operation (for conditional branches or comments). |



## Start

**`Pulse.Start()`** is explained in a previous chapter, but for completeness sake, here's a quick recap.

Every flow definition needs to start with a call to `Pulse.Start()`.
This strongly types the values that the flow can receive.
In addition, the result of the call needs to be used in the select part of the LINQ expression.
This strongly types the flow itself.

**Example:**
```csharp
from anInt in Pulse.Start<int>() // <=
select anInt;
```


## Using

**`Pulse.Using(...)`** Assigns an `IArtery` to the flow context, and thus enables tracing. 

**Example:**
```csharp
var collector = new TheCollector<int>();
var flow =
    from anInt in Pulse.Start<int>()
    from _ in Pulse.Using(collector) // <= 
    from t in Pulse.Trace(anInt)
    select anInt;
```


## Trace

**`Pulse.Trace(...)`** Emits trace data unconditionally to the current artery.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.Trace(anInt) // <=
select anInt;
```


## TraceIf

**`Pulse.TraceIf(...)`** Emits trace data conditionally, based on a boolean flag.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.TraceIf(anInt != 42, anInt) // <=
select anInt;
```


## Gather

**`Pulse.Gather(...)`** Binds a mutable box into flow memory (first write wins).

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1) // <=
select anInt;
```
**Warning:** `Gather` is thread-hostile by design, like a chainsaw. Use accordingly.  
Useful, powerful, and absolutely the wrong tool to wield in a multithreaded environment.


## Effect

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


## EffectIf

**`Pulse.EffectIf(...)`** Same as above, but conditional. 

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from seen42 in Pulse.Gather(false)
from eff in Pulse.EffectIf(anInt == 42, () => seen42.Value = true) // <=
select anInt;
```


## ToFlow

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


## ToFlowIf

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


## NoOp

**`Pulse.NoOp(...)`** A do-nothing operation (useful for conditional branches). 

**Example:**
```csharp
from anInt in Pulse.Start<int>()
    from _ in Pulse
        .NoOp(/* --- Also useful for Comments --- */)
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


## Pulse
**`Signal.Pulse(...)`** is the only way a flow can be instructed to do useful work.
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


## Set Artery
**`Signal.SetArtery(...)`** is used to inject an `IArtery` into the flow.
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` calls will be received by this .

A full example of this can be found at the end of the 'Building a Flow' chapter.


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
trace output is : `42 : 0`.

If we then call `Manipulate` like so: `signal.Manipulate<int>(a => a + 1);`, the next pulse: `signal.Pulse(42);`,
produces `42 : 1`.  


**Warning:** `Manipulate` mutates state between pulses. Sharp tool, like a scalpel.
Don't cut yourself.


## Scoped
**`Signal.Scoped(...)`** is sugaring for 'scoped' usage of the `Manipulate` method.

Given the same setup as before, we can write :

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


## Recap
State manipulation occurs before flow evaluation. Scoped reverses it afterward.
```
                     +-----------------------------+
Input via            |     Signal<T> instance      |
Signal.Pulse(x) ---> |  (wraps Flow<T> + state)    |
                     +-------------┬---------------+
                                   │
                      .------------+-------------.
                     /                          \
          Scoped / Manipulate                Normal Flow
        (adjust state before)               (start as-is)
                     \                          /
                      '------------┬-----------'
                                   ▼
                      +------------------------+
                      |    Flow<T> via LINQ    |
                      | (Start → Gather → ...) |
                      +------------------------+
                                   │
                  +----------------+----------------+
                  |                |                |
                  ▼                ▼                ▼
            +----------+     +-----------+     +-----------+
            | Gather() |     | Trace()   |     | ToFlow()  |
            | (state)  |     | (emit)    |     | (subflow) |
            +----------+     +-----------+     +-----------+
                                   │
                                   ▼
                        +------------------+
                        | Artery (optional) |
                        | Receives traces   |
                        +------------------+

```


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
        let indent = new string(' ', Math.Max(0, diagnosis.PhaseLevel) * 4)
        from log in Pulse.TraceIf(needsLogging, $"{indent}{diagnosis.Tags.First()}:{diagnosis.Message}")
        select diagnosis;
    return Signal.From(flow); ;
}
```


## Rendering this Document

```csharp
public static Flow<DocAttribute> RenderMarkdown =
    from doc in Pulse.Start<DocAttribute>()
    from previousLevel in Pulse.Gather(0)
    let headingLevel = doc.Order.Split('-').Length
    from first in Pulse.Gather(true)
    from rcaption in Pulse
        .NoOp(/* ---------------- Render Caption  ---------------- */ )
    let caption = doc.Caption
    let hasCaption = !string.IsNullOrEmpty(doc.Caption)
    let headingMarker = new string('#', headingLevel)
    let captionLine = $"{headingMarker} {caption}"
    from _t2 in Pulse.TraceIf(hasCaption, captionLine)
    from rcontent in Pulse
        .NoOp(/* ---------------- Render content  ---------------- */ )
    let content = doc.Content
    let hasContent = !string.IsNullOrEmpty(content)
    from _t3 in Pulse.TraceIf(hasContent, content, "")
    from end in Pulse
        .NoOp(/* ---------------- End of content  ---------------- */ )
    select doc;
```


## Transforming Markdown to Json

I'd advise against doing the following, but it _is_ possible. 
*This example exists to test QuickPulse's limits, **not as a recommendation**.  
For real work, use a proper markdown parser like [Markdig](https://github.com/xoofx/markdig).* 
```csharp
var json =
    from intAndTextAndBool in Pulse.Start<((int, string), bool)>()
    let intAndText = intAndTextAndBool.Item1
    let escaped = intAndText.Item2.Replace("\"", "\\\"")
    let comma = !intAndTextAndBool.Item2 ? ", " : "[ "
    from lb in Pulse.Trace($"{comma}{{ \"id\": {intAndText.Item1}, \"text\": \"{escaped}\" }}")
    select intAndTextAndBool;

var question =
    from input in Pulse.Start<string>()
    from isFirstQuestion in Pulse.Gather(true)
    from trimmed in Pulse.Gather("") // reuse later for tail
    from _ in Pulse.Effect(() => trimmed.Value = input.Trim())
    let i = trimmed.Value.TakeWhile(char.IsDigit).Count()
    let hasDot = i > 0 && i < trimmed.Value.Length && trimmed.Value[i] == '.'
    let numberText = i > 0 ? trimmed.Value.Substring(0, i) : null
    let rest = i + 1 < trimmed.Value.Length
        ? new string(trimmed.Value.Skip(i + 1).ToArray()).Trim().Replace("*", "")
        : ""
    let numberAndTextOrNull = int.TryParse(numberText, out var number)
        ? new (int, string)?((number, rest))
        : null
    let isQuestion = numberAndTextOrNull != null
    from flowed in Pulse.ToFlowIf(isQuestion, json, () => (numberAndTextOrNull.Value, isFirstQuestion.Value))
    from effect in Pulse.EffectIf(isQuestion, () => isFirstQuestion.Value = false)
    select input;

var flow =
    from start in Pulse.Start<string[]>()
    from questions in Pulse.ToFlow(question, start)
    from rb in Pulse.Trace("]")
    select start;
```
**Input:**
```
### Rivered 
**When the last card drops ...**

1. **Heb je ooit iets proberen te maken of repareren met een YouTube-tutorial? Wat was het?**  
*Tags: praktisch, zelfredzaamheid*  
*Facilitator note: Goed om zelfstandigheid en digitale leercurves aan te raken.*

2. **Wat is iets dat je hebt gemaakt (digitaal of fysiek) waar je trots op was, ook als het niet werkte?**  
*Tags: creatief, zelfexpressie*  
*Facilitator note: Helpt deelnemers zichzelf als makers te zien.*

3. **Als je het woord "algoritme" aan een kind moest uitleggen, wat zou je zeggen?**  
*Tags: technisch, abstract denken*  
*Facilitator note: Laat denkniveau en affiniteit met tech-taal zien.*
```
**Output:**
```json
[ { "id": 1, "text": "Heb je ooit iets proberen te maken of repareren met een YouTube-tutorial? Wat was het?" }
, { "id": 2, "text": "Wat is iets dat je hebt gemaakt (digitaal of fysiek) waar je trots op was, ook als het niet werkte?" }
, { "id": 3, "text": "Als je het woord \"algoritme\" aan een kind moest uitleggen, wat zou je zeggen?" }
]
```


# Why QuickPulse Exists
*A.k.a. A deep dark forest, a looking glass, and a trail of dead generators.*

A little while back I was writing a test for a method that took some JSON as input.
I got out my fuzzers out and went to work. And then... my fuzzers gave up.

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


