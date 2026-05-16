namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Emits a trace when the flag is true. 
    /// Use for simple conditional logging or signaling.
    /// </summary>
    public static Flow<Flow> TraceIf(bool flag, Func<object> data) =>
        Emit(Flag(flag), _ => data(), IntoArtery);

    /// <summary>
    /// Conditionally emits a formatted representation of the value currently carried
    /// by the flow into the current artery.
    /// </summary>
    public static Flow<Flow> TraceIf<T>(this Flow<T> other, bool flag, Func<T, object> formatter) =>
        other.SelectMany(a => TraceIf(flag, () => formatter(a)));

    /// <summary>
    /// Conditionally emits the value currently carried by the flow into the current artery.
    /// Use to trace the carried value unchanged when the flag is true.
    /// </summary>
    public static Flow<Flow> TraceIf<T>(this Flow<T> other, bool flag) =>
        other.SelectMany(a => TraceIf(flag, () => a!));

    /// <summary>
    /// Conditionally emits a formatted representation of the value currently carried
    /// by the flow into the current artery when the predicate is true.
    /// </summary>
    public static Flow<Flow> TraceIf<T>(this Flow<T> other, Func<T, bool> predicate, Func<T, object> formatter) =>
        other.SelectMany(a => TraceIf(predicate(a), () => formatter(a)));

    /// <summary>
    /// Conditionally emits the value currently carried by the flow into the current artery.
    /// Use to trace the carried value unchanged when the predicate is true.
    /// </summary>
    public static Flow<Flow> TraceIf<T>(this Flow<T> other, Func<T, bool> predicate) =>
        other.SelectMany(a => TraceIf(predicate(a), () => a!));
}
