namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Emits the given objects into the current artery. 
    /// Use to record static traces or messages.
    /// </summary>
    public static Flow<Flow> Trace(params object[] data) =>
        Emit(Always, _ => data, IntoArtery);

    /// <summary>
    /// Emits the given objects into the current artery. 
    /// Use to record static traces or messages.
    /// </summary>
    public static Flow<Flow> Trace(this Flow<Flow> other, params object[] data) =>
        other.Then(Trace(data));

    /// <summary>
    /// Continues the flow by emitting a value derived from the previous flow result
    /// into the current artery.
    /// Use to trace the value currently being carried by the flow.
    /// </summary>
    public static Flow<Flow> Trace<T>(this Flow<T> other, Func<T, object> formatter) =>
        other.SelectMany(a => Trace(formatter(a)));
}
