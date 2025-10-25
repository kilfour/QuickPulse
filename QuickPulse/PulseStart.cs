using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Begins a new flow by pulling the current input value from the signal. Use as the entry point in every LINQ flow.
    /// </summary>
    public static Flow<TOut> Start<TOut>() =>
        s => Cask.Some(s, s.GetValue<TOut>());

    /// <summary>
    /// Runs a subflow derived from the current input and then continues with the original input. Use for side-effects, tracing, or auxiliary work without changing T.
    /// </summary>
    public static Flow<T> Start<T>(Func<T, Flow<Unit>> step) =>
        Start<T, Unit>(step);

    /// <summary>
    /// Runs a subflow (returning any inner type) and discards its result, passing the original input downstream. Use when you need the behavior of the subflow, not its value.
    /// </summary>
    public static Flow<T> Start<T, TInner>(Func<T, Flow<TInner>> step) =>
        from x in Start<T>()
        from _ in step(x)
        select x;
}
