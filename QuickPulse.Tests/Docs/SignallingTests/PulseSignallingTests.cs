using QuickPulse.Arteries;
using QuickPulse.Instruments;
using QuickPulse.Tests._Tools;

namespace QuickPulse.Tests.Docs.SignallingTests;


[Doc(Order = Chapters.Signalling, Caption = "Pulsing a Flow: One Signal, One State", Content =
@"
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
")]
public class PulseSignallingTests
{
    [Doc(Order = Chapters.Signalling + "-1", Caption = "From", Content =
@"
**`Signal.From(...)`** is a simple factory method used to get hold of a `Signal<T>` instance
that wraps the passed in `Flow<T>`.


**Example:**
```csharp
from anInt in Pulse.Start<int>()
select anInt;
var signal = Signal.From(flow);
```
")]
    [Fact]
    public void Signal_from()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            select anInt;
        var signal = Signal.From(flow);
        Assert.IsType<Signal<int>>(signal);
    }

    [Doc(Order = Chapters.Signalling + "-1.5", Caption = "Tracing", Content =
@"
**`Signal.Tracing(...)`** is sugaring for: 
**Example:**
```csharp
var flow =
    from start in Pulse.Start<T>()
    from _ in Pulse.Trace(start)
    select start;
return new Signal<T>(flow);
```
Useful if you want to just quickly grab a tracer.
")]
    [Fact]
    public void Signal_tracing()
    {
        var signal = Signal.Tracing<int>();
        var collector = signal.SetAndReturnArtery(new TheCollector<int>());
        signal.Pulse(42);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.Signalling + "-2", Caption = "Pulse", Content =
@"**`Signal.Pulse(...)`** is the main way a flow can be instructed to do useful work.
In its simplest form this looks like the following.
**Example:**
```csharp
from anInt in Pulse.Start<int>()
select anInt;
var signal = Signal.From(flow);
signal.Pulse(42);
```
This sends the int `42` into the flow.
")]
    [Fact]
    public void Signal_pulse()
    {
        List<int> collector = [];
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Effect(() => collector.Add(anInt))
            select anInt;
        var signal = Signal.From(flow);
        signal.Pulse(42);
        Assert.Single(collector);
        Assert.Equal(42, collector[0]);
    }

    [Doc(Order = Chapters.Signalling + "-2-1", Content =
@"The argument of this method is actually `params T[] input`, so you can send multiple values in, in one call.
**Example:**
```csharp
signal.Pulse(42, 43, 44);
```
This will execute the flow three times, once for each value passed in.
")]
    [Fact]
    public void Signal_pulse_thrice()
    {
        List<int> collector = [];
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Effect(() => collector.Add(anInt))
            select anInt;
        var signal = Signal.From(flow);
        signal.Pulse(42, 43, 44);
        Assert.Equal(3, collector.Count);
        Assert.Equal(42, collector[0]);
        Assert.Equal(43, collector[1]);
        Assert.Equal(44, collector[2]);
    }

    [Doc(Order = Chapters.Signalling + "-2-2", Content =
@"For ease of use, when dealing with `IEnumerable` return values from various sources,
an overload exists: `Pulse(IEnumerable<T> inputs)`. 
**Example:**
```csharp
signal.Pulse(new List<int> { 42, 43, 44 });
```
This behaves exactly like the previous example.
")]
    [Fact]
    public void Signal_pulse_thrice_enumerable()
    {
        List<int> collector = [];
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Effect(() => collector.Add(anInt))
            select anInt;
        var signal = Signal.From(flow);
        signal.Pulse(new List<int> { 42, 43, 44 });
        Assert.Equal(3, collector.Count);
        Assert.Equal(42, collector[0]);
        Assert.Equal(43, collector[1]);
        Assert.Equal(44, collector[2]);
    }

    [Doc(Order = Chapters.Signalling + "-3", Caption = "Pulse Multiple", Content =
@"**`Signal.PulseMultiple(...)`** is a helper method that sugars a `for(int i = ...)` type structure.
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
")]
    [Fact]
    public void Signal_pulse_Multiple()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from g in Pulse.Gather(0)
            from t in Pulse.Trace(anInt + g.Value)
            from e in Pulse.Effect(() => g.Value++)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.PulseMultiple(3, 40);
        Assert.Equal(3, collector.TheExhibit.Count);
        Assert.Equal(40, collector.TheExhibit[0]);
        Assert.Equal(41, collector.TheExhibit[1]);
        Assert.Equal(42, collector.TheExhibit[2]);
    }

    [Doc(Order = Chapters.Signalling + "-4", Caption = "Pulse Until", Content =
@"**`Signal.PulseUntil(...)`** is a helper method that sugars a `while(...)` type structure.
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
")]
    [Fact]
    public void Signal_pulse_until()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from g in Pulse.Gather(0)
            from t in Pulse.Trace(anInt + g.Value)
            from e in Pulse.Effect(() => g.Value++)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.PulseUntil(() => collector.TheExhibit.Contains(42), 40);
        Assert.Equal(3, collector.TheExhibit.Count);
        Assert.Equal(40, collector.TheExhibit[0]);
        Assert.Equal(41, collector.TheExhibit[1]);
        Assert.Equal(42, collector.TheExhibit[2]);
    }

    [Doc(Order = Chapters.Signalling + "-4-1", Content =
@"**Warning:** Make sure you stop pulsing. `Signal.PulseUntil(...)` throws an exception if you try to pulse over 256 times.
")]
    [Fact]
    public void Signal_pulse_until_exception()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from g in Pulse.Gather(0)
            from t in Pulse.Trace(g.Value)
            from e in Pulse.Effect(() => g.Value++)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        var exception = Assert.Throws<ComputerSaysNo>(() => signal.PulseUntil(() => false, 0));
        Assert.Equal(255, collector.TheExhibit.Last());
    }

    [Doc(Order = Chapters.Signalling + "-5", Caption = "Pulse Multiple Until", Content =
@"**`Signal.PulseUntil(...)`** is a combination of the previous two methods.
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
")]
    [Fact]
    public void Signal_pulse_multiple_until()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from g in Pulse.Gather(0)
            from t in Pulse.Trace(anInt + g.Value)
            from e in Pulse.Effect(() => g.Value++)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.PulseMultipleUntil(3, () => false, 40);
        Assert.Equal(3, collector.TheExhibit.Count);
        Assert.Equal(40, collector.TheExhibit[0]);
        Assert.Equal(41, collector.TheExhibit[1]);
        Assert.Equal(42, collector.TheExhibit[2]);
    }

    [Doc(Order = Chapters.Signalling + "-5-1", Content =
    @"But if the condition supplied is satisfied it will stop pulsing early.
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
")]
    [Fact]
    public void Signal_pulse_multiple_until_early_exit()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from g in Pulse.Gather(0)
            from t in Pulse.Trace(anInt + g.Value)
            from e in Pulse.Effect(() => g.Value++)
            select anInt;
        var signal = Signal.From(flow).SetArtery(collector);
        signal.PulseMultipleUntil(3, () => collector.TheExhibit.Contains(40), 40);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(40, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.Signalling + "-6", Caption = "Set Artery", Content =
@"**`Signal.SetArtery(...)`** is used to inject an `IArtery` into the flow.
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` calls will be received by this .

A full example of this can be found at the end of the 'Building a Flow' chapter.
")]
    [Fact]
    public void Signal_set_artery()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Trace(anInt)
            select anInt;
        var signal = Signal.From(flow);
        signal.SetArtery(collector);
        signal.Pulse(42);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.Signalling + "-7", Caption = "Set And Return Artery", Content =
@"**`Signal.SetAndReturnArtery(...)`** is the same as above, but instead of returning the signal it returns the artery.
```csharp
var collector = signal.SetAndReturnArtery(new TheCollector<int>());
```
")]
    [Fact]
    public void Signal_set_and_return_artery()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Trace(anInt)
            select anInt;
        var signal = Signal.From(flow);
        var collector = signal.SetAndReturnArtery(new TheCollector<int>());
        signal.Pulse(42);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Doc(Order = Chapters.Signalling + "-8", Caption = "Manipulate", Content =
@"**`Signal.Manipulate(...)`** is used in conjunction with `Pulse.Gather(...)`,
and allows for manipulating the flow in between pulses.
**Given this setup:**
```csharp
 var flow =
    from anInt in Pulse.Start<int>()
    from gathered in Pulse.Gather(0)
    from _ in Pulse.Trace($""{anInt} : {gathered.Value}"")
    select anInt;
var signal = Signal.From(flow);
```
And we pulse once like so: `signal.Pulse(42);` the flow will gather the input in the gathered range variable and
trace output is: `42 : 0`.

If we then call `Manipulate` like so: `signal.Manipulate<int>(a => a + 1);`, the next pulse: `signal.Pulse(42);`,
produces `42 : 1`.  


**Warning:** `Manipulate` mutates state between pulses. Sharp tool, like a scalpel.
Don't cut yourself.
")]
    [Fact]
    public void Signal_set_manipulate()
    {
        var collector = new TheCollector<string>();
        var flow =
            from anInt in Pulse.Start<int>()
            from gathered in Pulse.Gather(0)
            from _ in Pulse.Trace($"{anInt} : {gathered.Value}")
            select anInt;
        var signal = Signal.From(flow);
        signal.SetArtery(collector);
        signal.Pulse(42);
        signal.Manipulate<int>(a => a + 1);
        signal.Pulse(42);
        Assert.Equal(2, collector.TheExhibit.Count);
        Assert.Equal("42 : 0", collector.TheExhibit[0]);
        Assert.Equal("42 : 1", collector.TheExhibit[1]);
    }

    [Doc(Order = Chapters.Signalling + "-9", Caption = "Scoped", Content =
@"**`Signal.Scoped(...)`** is sugaring for 'scoped' usage of the `Manipulate` method.

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
")]
    [Fact]
    public void Signal_set_scoped()
    {
        var collector = new TheCollector<string>();
        var flow =
            from anInt in Pulse.Start<int>()
            from gathered in Pulse.Gather(0)
            from _ in Pulse.Trace($"{anInt} : {gathered.Value}")
            select anInt;
        var signal = Signal.From(flow);
        signal.SetArtery(collector);
        signal.Pulse(42);
        using (signal.Scoped<int>(a => a + 1, a => a - 1))
        {
            signal.Pulse(42);
        }
        signal.Pulse(42);
        Assert.Equal(3, collector.TheExhibit.Count);
        Assert.Equal("42 : 0", collector.TheExhibit[0]);
        Assert.Equal("42 : 1", collector.TheExhibit[1]);
        Assert.Equal("42 : 0", collector.TheExhibit[2]);
    }

    [Doc(Order = Chapters.Signalling + "-10", Caption = "Recap", Content =
@"State manipulation occurs before flow evaluation. Scoped reverses it afterward.
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
")]
    [Fact]
    public void Signal_recap() { /* Place holder*/ }
}