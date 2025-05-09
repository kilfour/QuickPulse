using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Tests._Tools;

namespace QuickPulse.Tests.BuildFlowTests;


[Doc(Order = Chapters.Signalling, Caption = "Signalling", Content =
@"")]
public class PulseSignallingTests
{
    [Doc(Order = Chapters.Signalling + "-1", Caption = "From", Content =
@"
**`Signal.From<T>(Flow<T> flow)`** is a simple factory method used to get hold of a `Signal<T>` instance
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

    [Doc(Order = Chapters.Signalling + "-2", Caption = "Pulse", Content =
@"**`Signal<T>.Pulse(...)`** is the only way a flow can be instructed to do useful work.
**Todo:** *Explain how signal wraps state.*
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

    [Doc(Order = Chapters.Signalling + "-3", Caption = "Set Artery", Content =
@"**`Signal<T> SetArtery(IArtery artery)`** is used to inject an `IArtery` into the flow.
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` calls will be received by this .

A full example of this can be found at the end of the 'Building a Flow' chapter.
")]
    [Fact]
    public void Signal_set_artery() //Manipulate // Scoped
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
}