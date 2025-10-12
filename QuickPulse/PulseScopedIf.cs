namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Temporarily replaces a stored value when the flag is true, restoring it afterward. Use for conditional scoped overrides.
    /// </summary>
    public static Flow<Unit> ScopedIf<TValue>(bool flag, Func<TValue, TValue> enter, Flow<Unit> flow) =>
        Runnel(Flag(flag), WithScope(enter, flow));

    /// <summary>
    /// Temporarily replaces a stored value when the predicate is true for the current state, restoring it afterward. Use for context-driven scoped overrides.
    /// </summary>
    public static Flow<Unit> ScopedIf<TValue, TBox>(Func<TBox, bool> predicate, Func<TValue, TValue> enter, Flow<Unit> flow) =>
        Runnel(Sluice(predicate), WithScope(enter, flow));
}
