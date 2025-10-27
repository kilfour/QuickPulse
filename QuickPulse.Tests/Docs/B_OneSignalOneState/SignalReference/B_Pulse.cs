using QuickPulse.Arteries;
using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.B_OneSignalOneState.SignalReference;

public class B_Pulse
{
    [Fact]
    [DocHeader("Pulsing One Value")]
    [DocContent("`Signal.Pulse(...)` is the main way a flow can be instructed to do useful work.")]
    [DocExample(typeof(B_Pulse), nameof(Signal_pulse_example))]
    [DocContent("This sends the int's `42`, `43` and `44` into the flow.")]
    public void Signal_pulse()
    {
        var collector = TheCollector.Exhibits<int>();
        Signal_pulse_example(collector);
        Assert.Equal(3, collector.TheExhibit.Count);
        Assert.Equal(42, collector.TheExhibit[0]);
        Assert.Equal(43, collector.TheExhibit[1]);
        Assert.Equal(44, collector.TheExhibit[2]);
    }

    [CodeSnippet]
    [CodeRemove(".SetArtery(collector)")]
    private static void Signal_pulse_example(Collector<int> collector)
    {
        Signal.From<int>(a => Pulse.Trace(a))
            .SetArtery(collector)
            .Pulse(42)
            .Pulse(43)
            .Pulse(44);
    }

    [Fact]
    [DocHeader("Pulsing Many Values")]
    [DocContent("For ease of use, when dealing with `IEnumerable` return values from various sources, an overload exists: `Signal.Pulse(IEnumerable<T> inputs)`. ")]
    [DocExample(typeof(B_Pulse), nameof(Signal_pulse_thrice_enumerable_example))]
    [DocContent("Same behaviour as the single pulse example.")]
    public void Signal_pulse_thrice_enumerable()
    {
        var collector = TheCollector.Exhibits<int>();
        Signal_pulse_thrice_enumerable_example(collector);
        Assert.Equal(3, collector.TheExhibit.Count);
        Assert.Equal(42, collector.TheExhibit[0]);
        Assert.Equal(43, collector.TheExhibit[1]);
        Assert.Equal(44, collector.TheExhibit[2]);
    }

    [CodeSnippet]
    [CodeRemove(".SetArtery(collector)")]
    private static void Signal_pulse_thrice_enumerable_example(Collector<int> collector)
    {
        Signal.From<int>(a => Pulse.Trace(a))
            .SetArtery(collector)
            .Pulse([42, 43, 44]);
    }

    [Fact]
    [DocHeader("Pulsing Nothing")]
    [DocContent(
@"Lastly, in some rare circumstances, a flow does not take any input. In `QuickPulse` *nothing* is represented by a `Flow` type.  
So in order to advance a flow of type `Flow<Flow>` you can use the `Signal.Pulse()` overload.")]
    [DocExample(typeof(B_Pulse), nameof(Signal_pulse_unit_example))]
    public void Signal_pulse_unit()
    {
        var collector = TheCollector.Exhibits<int>();
        Signal_pulse_unit_example(collector);
        Assert.Equal(3, collector.TheExhibit.Count);
        Assert.Equal(42, collector.TheExhibit[0]);
        Assert.Equal(43, collector.TheExhibit[1]);
        Assert.Equal(44, collector.TheExhibit[2]);
    }

    [CodeSnippet]
    [CodeRemove(".SetArtery(collector)")]
    private static void Signal_pulse_unit_example(Collector<int> collector)
    {
        var flow =
            from _ in Pulse.Start<Flow>()
            from _1 in Pulse.Prime(() => 42)
            from _2 in Pulse.Trace<int>(a => a)
            from _3 in Pulse.Manipulate<int>(a => a + 1)
            select Flow.Continue;
        Signal.From(flow)
            .SetArtery(collector)
            .Pulse()
            .Pulse()
            .Pulse();
        // This one also results in [42, 43, 44]
    }
}