namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Temporarily replaces a stored value when the flag is true, restoring it afterward. Use for conditional scoped overrides.
    /// </summary>
    public static Flow<Flow> ScopedIf<TCell>(bool flag, Func<TCell, TCell> enter, Flow<Flow> flow) =>
        Emit(Flag(flag), WithScope(enter, flow));

    /// <summary>
    /// Temporarily replaces a stored value when the flag is true, restoring it afterward. Use for conditional scoped overrides.
    /// </summary>
    public static Flow<Flow> ScopedIf<TCell>(this Flow<Flow> other, bool flag, Func<TCell, TCell> enter, Flow<Flow> flow) =>
        other.Then(ScopedIf(flag, enter, flow));

    /// <summary>
    /// Temporarily replaces a stored value when the predicate is true for the current state, restoring it afterward. Use for context-driven scoped overrides.
    /// </summary>
    public static Flow<Flow> ScopedIf<TCell>(Func<TCell, bool> predicate, Func<TCell, TCell> enter, Flow<Flow> flow) =>
        Emit(Gate(predicate), WithScope(enter, flow));

    /// <summary>
    /// Temporarily replaces a stored value when the predicate is true for the current state, restoring it afterward. Use for context-driven scoped overrides.
    /// </summary>
    public static Flow<Flow> ScopedIf<TCell>(this Flow<Flow> other, Func<TCell, bool> predicate, Func<TCell, TCell> enter, Flow<Flow> flow) =>
        other.Then(ScopedIf(predicate, enter, flow));
}
