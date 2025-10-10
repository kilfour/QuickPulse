using QuickPulse.Explains.Deprecated;
using QuickPulse.Arteries;
using QuickPulse.Instruments;
using QuickPulse.Arteries.Shunt;
using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.C_OneSignalOneState;

[DocFile]
[DocFileHeader("Pulsing a Flow: One Signal, One State")]
[DocContent(
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
public class OneSignalOneStateTests
{
    [DocHeader("From")]
    [DocContent(
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

    [DocHeader("Tracing")]
    [DocContent(
@"
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

    [DocHeader("Pulse")]
    [DocContent(
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
        signal.SetArtery(Install.Shunt).Pulse(42);
        Assert.Single(collector);
        Assert.Equal(42, collector[0]);
    }

    [DocContent(
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
        var signal = Signal.From(flow).SetArtery(Install.Shunt);
        signal.Pulse(new List<int> { 42, 43, 44 });
        Assert.Equal(3, collector.Count);
        Assert.Equal(42, collector[0]);
        Assert.Equal(43, collector[1]);
        Assert.Equal(44, collector[2]);
    }

    [Fact(Skip = "doc this")]
    public void Signal_pulse_null()
    {
        List<int> collector = [];
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Effect(() => collector.Add(anInt))
            select anInt;
        var signal = Signal.From(flow).SetArtery(Install.Shunt);
        signal.Pulse((List<int>)null!);
    }

    [DocHeader("Set Artery")]
    [DocContent(
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

    [DocHeader("Set And Return Artery")]
    [DocContent(
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

    [DocHeader("Get Artery")]
    [DocContent(
@"**`Signal.GetArtery<TArtery>(...)`** can be used to retrieve the current `IArtery` set on the signal.
**Example:**
```csharp
var signal = Signal.Tracing<int>().SetArtery(new TheCollector<int>()).Pulse(42);

var collector = signal.GetArtery<TheCollector<int>>()!;
Assert.Single(collector.TheExhibit);
Assert.Equal(42, collector.TheExhibit[0]);
```
")]
    [Fact]
    public void Signal_get_artery()
    {
        var signal = Signal.Tracing<int>().SetArtery(new TheCollector<int>()).Pulse(42);
        var collector = signal.GetArtery<TheCollector<int>>();
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [DocContent(
@"**`Signal.GetArtery<TArtery>(...)`** throws if no `IArtery` is currently set on the `Signal`.
")]
    [Fact]
    public void Signal_get_artery_throws_if_no_artery_set()
    {
        var ex = Assert.Throws<NullReferenceException>(() => Signal.Tracing<int>().GetArtery<TheCollector<int>>());
        Assert.Equal("Object reference not set to an instance of an object.", ex.Message);
    }

    [DocContent(
@"**`Signal.GetArtery<TArtery>(...)`** throws if trying to retrieve the wrong type of `IArtery`.
")]
    [Fact]
    public void Signal_get_artery_throws_if_wrong_typed_retrieved()
    {
        var ex = Assert.Throws<ComputerSaysNo>(() => Signal.Tracing<int>().SetArtery(TheString.Catcher()).GetArtery<TheCollector<int>>());
        Assert.Equal("No IArtery set on the current Signal.", ex.Message);
    }
}