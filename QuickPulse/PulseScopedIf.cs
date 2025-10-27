namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Temporarily replaces a stored value when the flag is true, restoring it afterward. Use for conditional scoped overrides.
    /// </summary>
    public static Flow<Flow> ScopedIf<TValue>(bool flag, Func<TValue, TValue> enter, Flow<Flow> flow) =>
        Emit(Flag(flag), WithScope(enter, flow));

    /// <summary>
    /// Temporarily replaces a stored value when the predicate is true for the current state, restoring it afterward. Use for context-driven scoped overrides.
    /// </summary>
    public static Flow<Flow> ScopedIf<TValue, TCell>(Func<TCell, bool> predicate, Func<TValue, TValue> enter, Flow<Flow> flow) =>
        Emit(Gate(predicate), WithScope(enter, flow));
}
