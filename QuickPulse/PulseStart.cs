using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Begins a new flow by pulling the current input value from the signal. 
    /// Use as the entry point in every LINQ flow.
    /// </summary>
    public static Flow<TValue> Start<TValue>() =>
        s => Beat.Some(s, s.GetValue<TValue>());

    /// <summary>
    /// Runs a subflow derived from the current input and then continues with the original input.
    /// Use for side-effects, tracing, or auxiliary work without changing TValue.
    /// </summary>
    public static Flow<TValue> Start<TValue>(Func<TValue, Flow<Flow>> step) =>
        Start<TValue, Flow>(step);

    /// <summary>
    /// Runs a subflow (returning any inner type) and discards its result, passing the original input downstream. 
    /// Use when you need the behavior of the subflow, not its value.
    /// </summary>
    public static Flow<TValue> Start<TValue, TInner>(Func<TValue, Flow<TInner>> step) =>
        from x in Start<TValue>()
        from _ in step(x)
        select x;
}
