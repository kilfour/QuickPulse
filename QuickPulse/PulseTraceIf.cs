namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Emits a trace when the flag is true. Use for simple conditional logging or signaling.
    /// </summary>
    public static Flow<Flow> TraceIf(bool flag, Func<object> data) =>
        Runnel(Flag(flag), _ => data(), IntoArtery);

    /// <summary>
    /// Emits a trace when the predicate evaluates to true for the current state. Use for state-aware conditional traces.
    /// </summary>
    public static Flow<Flow> TraceIf<T>(Func<T, bool> predicate, Func<object> data) =>
        Runnel(Sluice(predicate), _ => data(), IntoArtery);

    /// <summary>
    /// Emits a trace derived from the current state when the predicate is true. Use for context-sensitive diagnostic output.
    /// </summary>
    public static Flow<Flow> TraceIf<T>(Func<T, bool> predicate, Func<T, object> extractor) =>
        Runnel(Sluice(predicate), ExtractDataFromBox(extractor), IntoArtery);
}
