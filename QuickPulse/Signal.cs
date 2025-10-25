using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

/// <summary>
/// Entry point for creating live signal instances from flows.
/// </summary>
public static class Signal
{
    /// <summary>
    /// Creates a new signal from a predefined flow. Use when you already have a composed Flow&lt;T&gt;.
    /// </summary>
    public static Signal<T> From<T>(Flow<T> flow) => new(flow);

    /// <summary>
    /// Creates a new signal from a flow factory. Use to inline or lazily construct small flows.
    /// </summary>
    public static Signal<T> From<T>(Func<T, Flow<Flow>> flowFactory) =>
        new(Pulse.GetFlowFromFactory(flowFactory));
}

/// <summary>
/// A live, stateful instance of a flow that reacts to incoming pulses.
/// </summary>
public class Signal<T>(Flow<T> flow)
{
    private readonly State state = new();
    private readonly Flow<T> flow = flow;

    /// <summary>
    /// Indicates whether the underlying flow has terminated and no longer accepts input.
    /// </summary>
    public bool FlowRanDry => state.FlowRanDry;

    /// <summary>
    /// Pulses a flow that takes no input (Flow&lt;Flow&gt;). Advances the flow by one step.
    /// </summary>
    public Signal<T> Pulse()
        => typeof(T) == typeof(Flow)
            ? Pulse((T)(object)Flow.Continue)
            : ComputerSays.No<Signal<T>>("Pulse() without arguments only allowed for Signal<Flow>.");

    private static IEnumerable<T> Single(T value) { yield return value; }

    /// <summary>
    /// Sends a single value through the flow. Use for simple one-off pulses.
    /// </summary>
    public Signal<T> Pulse(T value) => Pulse(Single(value));

    /// <summary>
    /// Sends multiple values through the flow in sequence. Preserves internal state between pulses.
    /// </summary>
    public Signal<T> Pulse(IEnumerable<T> inputs)
    {
        if (FlowRanDry) return this;
        foreach (var item in inputs)
        {
            flow(state.SetValue(item));
            if (FlowRanDry) break;
        }
        return this;
    }

    /// <summary>
    /// Sets the main artery for this signal. Determines where emitted traces and outputs flow.
    /// </summary>
    public Signal<T> SetArtery(IArtery artery) => Chain.It(() => state.SetArtery(artery), this);

    /// <summary>
    /// Sets and returns the given artery. A convenient way to wire and capture an artery inline.
    /// </summary>
    public TArtery SetAndReturnArtery<TArtery>(TArtery artery) where TArtery : IArtery =>
        Chain.It(() => state.SetArtery(artery), artery);

    /// <summary>
    /// Retrieves the current artery of the specified type. Throws if not grafted or registered.
    /// </summary>
    public TArtery GetArtery<TArtery>() where TArtery : class, IArtery
        => state.GetArtery<TArtery>();

    /// <summary>
    /// Grafts an additional artery onto the signal. Use for diagnostics or side-channel tracing.
    /// </summary>
    public Signal<T> Graft<TArtery>(TArtery artery) where TArtery : IArtery
        => Chain.It(() => state.Graft(artery), this);

    /// <summary>
    /// Executes a finalizing flow once the signal has completed. Use for cleanup or summary actions.
    /// </summary>
    public Signal<T> FlatLine(Flow<Flow> finisher)
        => Chain.It(() => finisher(state), this);
}
