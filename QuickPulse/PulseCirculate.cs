namespace QuickPulse;

/// <summary>
/// Core combinators for composing, routing, and inspecting QuickPulse flows.
/// </summary>
public static partial class Pulse
{
    /// <summary>
    /// Runs a subflow derived from the current input and then continues with the original input. Use for side-effects, tracing, or auxiliary work without changing T.
    /// </summary>
    public static Flow<T> Circulate<T>(Func<T, Flow<Unit>> step) =>
        Circulate<T, Unit>(step);

    /// <summary>
    /// Runs a subflow (returning any inner type) and discards its result, passing the original input downstream. Use when you need the behavior of the subflow, not its value.
    /// </summary>
    public static Flow<T> Circulate<T, TInner>(Func<T, Flow<TInner>> step) =>
        from x in Start<T>()
        from _ in step(x)
        select x;
}
