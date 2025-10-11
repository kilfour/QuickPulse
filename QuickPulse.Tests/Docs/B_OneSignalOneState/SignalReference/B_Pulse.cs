using QuickPulse.Arteries;
using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.B_OneSignalOneState.SignalReference;

[DocFile]
public class B_Pulse
{
    [Fact]
    [DocContent("**`Signal.Pulse(...)`** is the main way a flow can be instructed to do useful work.")]
    [DocExample(typeof(B_Pulse), nameof(Signal_pulse))]
    [DocContent("As the `Assert`'s demonstrate, this sends the int `42` into the flow.")]
    [CodeSnippet]
    public void Signal_pulse()
    {
        var collector = TheCollector.Exhibits<int>();
        Signal.From<int>(a => Pulse.Trace(a))
            .SetArtery(collector)
            .Pulse(42);
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }


    [Fact]
    [DocContent("For ease of use, when dealing with `IEnumerable` return values from various sources, an overload exists: `Pulse(IEnumerable<T> inputs)`. ")]
    [DocExample(typeof(B_Pulse), nameof(Signal_pulse_thrice_enumerable))]
    [DocContent("This behaves exactly like the previous example.")]
    [CodeSnippet]
    public void Signal_pulse_thrice_enumerable()
    {
        var collector = TheCollector.Exhibits<int>();
        Signal.From<int>(a => Pulse.Trace(a))
            .SetArtery(collector)
            .Pulse([42, 43, 44]);
        Assert.Equal(3, collector.TheExhibit.Count);
        Assert.Equal(42, collector.TheExhibit[0]);
        Assert.Equal(43, collector.TheExhibit[1]);
        Assert.Equal(44, collector.TheExhibit[2]);
    }

    [Fact(Skip = "doc this")]
    public void Signal_pulse_null()
    {
        // List<int> collector = [];
        // var flow =
        //     from anInt in Pulse.Start<int>()
        //     from _ in Pulse.Effect(() => collector.Add(anInt))
        //     select anInt;
        // var signal = Signal.From(flow).SetArtery(Install.Shunt);
        // signal.Pulse((List<int>)null!);
    }
}