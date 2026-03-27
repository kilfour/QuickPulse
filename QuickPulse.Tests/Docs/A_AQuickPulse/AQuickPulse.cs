using QuickPulse.Explains;
using QuickPulse.Arteries;


namespace QuickPulse.Tests.Docs.A_AQuickPulse;

[DocFile]
[DocContent("To explain how QuickPulse works (not least to myself), let's build up a flow step by step.")]
public class AQuickPulse
{

    [DocHeader("A Mental Map", 1)]
    [DocContent(
@"Before diving deeper, it helps to understand the three pillars that make up QuickPulse's core.

1. `Flow<T>`: 
A flow is a *recipe for behaviour*. It defines how input values are transformed, traced, or manipulated.  
A flow itself doesn't do anything until it's pulsed.

2. `Signal<T>`: 
A signal is an *instance* of a flow. It evolves with each pulse.
Every signal carries its own internal state and provides the entry point for execution.
You can have multiple signals from the same flow.

3. `IArtery`: 
Arteries are the *output channels* of a signal. They collect, display, or record whatever the flow emits.")]
    private void MentalMap() { /* Placeholder */ }

    [Fact]
    [DocHeader("Doing Something with the Input")]
    [DocContent("Let's trace the values as they pass through:")]
    [DocExample(typeof(AQuickPulse), nameof(Adding_a_trace_example))]
    public void Adding_a_trace()
        => Assert.IsType<Func<int, Flow<Flow>>>(Adding_a_trace_example());

    [CodeSnippet]
    [CodeRemove("return")]
    private static Func<int, Flow<Flow>> Adding_a_trace_example()
    {
        return a => Pulse.Trace(a);
    }

    [DocHeader("Executing a Flow")]
    [DocContent(
@"To execute a flow, we need a `Signal<T>`, which is created via: `Signal.From<T>(Flow<T> flow)`.

Example:")]
    [DocExample(typeof(AQuickPulse), nameof(Adding_a_signal_example))]
    [Fact]
    public void Adding_a_signal()
        => Assert.IsType<Signal<int>>(Adding_a_signal_example());

    [CodeSnippet]
    [CodeRemove("return signal;")]
    private static Signal<int> Adding_a_signal_example()
    {
        var signal = Signal.From<int>(a => Pulse.Trace(a));
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
        Assert.NotNull(signal.GetArtery<NullArtery>());
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Signal<int> Adding_a_pulse_example()
    {
        return Signal.From<int>(a => Pulse.Trace(a))
            .Pulse(42);
    }

    [Fact]
    [DocHeader("Capturing the Trace")]
    [DocContent(
@"To observe what flows through, we can add an `IArtery` by using `SetArtery` directly on the signal.

Example:")]
    [DocExample(typeof(AQuickPulse), nameof(Adding_an_artery_example))]
    public void Adding_an_artery()
    {
        var collector = Adding_an_artery_example();
        Assert.Equal(3, collector.Values.Count);
        Assert.Equal(42, collector.Values[0]);
        Assert.Equal(43, collector.Values[1]);
        Assert.Equal(44, collector.Values[2]);
    }

    [CodeSnippet]
    [CodeRemove("return collector;")]
    private static Collector<int> Adding_an_artery_example()
    {
        var collector = Collect.ValuesOf<int>();
        Signal.From<int>(a => Pulse.Trace(a))
            .SetArtery(collector)
            .Pulse([42, 43, 44]);
        // collector.Values now holds => [42, 43, 44]."
        return collector;
    }
}