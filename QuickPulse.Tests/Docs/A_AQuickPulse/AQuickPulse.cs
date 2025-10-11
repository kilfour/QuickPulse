using QuickPulse.Explains;
using QuickPulse.Arteries;


namespace QuickPulse.Tests.Docs.A_AQuickPulse;

[DocFile]
[DocContent("To explain how QuickPulse works (not least to myself), let's build up a flow step by step.")]
public class AQuickPulse
{
    [DocHeader("The Minimal Flow")]
    [DocContent(
@"The type generic in `Pulse.Start<T>` defines the **input type** to the flow.  
**Note:** It is required to select the result of `Pulse.Start(...)` at the end of the LINQ chain for the flow to be considered well-formed.")]
    [Fact]
    [DocExample(typeof(AQuickPulse), nameof(Adding_a_trace_example))]
    public void Minimal_definition_start()
    {
        Assert.IsType<Flow<int>>(Minimal_definition_start_example());
    }

    [CodeSnippet]
    [CodeReplace("return", "")]
    private static Flow<int> Minimal_definition_start_example()
    {
        return
            from anInt in Pulse.Start<int>()
            select anInt;
    }

    [Fact]
    [DocHeader("Doing Something with the Input")]
    [DocContent("Let's trace the values as they pass through:")]
    [DocExample(typeof(AQuickPulse), nameof(Adding_a_trace_example))]
    public void Adding_a_trace()
        => Assert.IsType<Flow<int>>(Adding_a_trace_example());

    [CodeSnippet]
    [CodeReplace("return", "")]
    private static Flow<int> Adding_a_trace_example()
    {
        return
            from anInt in Pulse.Start<int>()
            from trace in Pulse.Trace(anInt)
            select anInt;
    }

    [DocHeader("Executing a Flow")]
    [DocContent(
@"To execute a flow, we need a `Signal<T>`, which is created via: `Signal.From<T>(Flow<T> flow)`.

Example:")]
    [Fact]
    public void Adding_a_signal()
        => Assert.IsType<Signal<int>>(Adding_a_signal_example());

    [CodeSnippet]
    [CodeReplace("return signal;", "")]
    private static Signal<int> Adding_a_signal_example()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            from trace in Pulse.Trace(anInt)
            select anInt;
        var signal = Signal.From(flow);
        return signal;
    }

    [DocHeader("Sending Values Through the Flow")]
    [DocContent(
@"Once you have a signal, you can push values into the flow by calling: `Signal.Pulse(...)`.

For example, sending the value `42` into the flow:")]
    [DocExample(typeof(AQuickPulse), nameof(Adding_a_pulse_example))]
    [Fact]
    public void Adding_a_pulse()
    {
        var signal = Adding_a_pulse_example();
        Assert.NotNull(signal.GetArtery<Shunt>());
    }

    [CodeSnippet]
    [CodeReplace("return", "")]
    private static Signal<int> Adding_a_pulse_example()
    {
        return Signal.From(
                from anInt in Pulse.Start<int>()
                from trace in Pulse.Trace(anInt)
                select anInt)
            .Pulse(42);
    }

    [Fact]
    [DocHeader("Capturing the Trace")]
    [DocContent(
@"To observe what flows through, we can add an `IArtery` by using `SetArtery` directly on the signal.

Example:")]
    [DocExample(typeof(AQuickPulse), nameof(Adding_an_artery))]
    [CodeExample]
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
}