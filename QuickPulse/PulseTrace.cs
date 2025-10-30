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
    /// Emits a value extracted from the current state into the artery. 
    /// Use for dynamic or computed traces.
    /// </summary>
    public static Flow<Flow> Trace<TCell>(Func<TCell, object> extractor) =>
        Emit(Always, ExtractDataFromCell(extractor), IntoArtery);

    /// <summary>
    /// Emits the current value of type <typeparamref name="TCell"/> from memory into the artery.  
    /// Use to trace stored state directly, without computing or transforming it.
    /// </summary>
    public static Flow<Flow> Trace<TCell>() =>
        Emit(Always, s => s.GetTheCell<TCell>().Value!, IntoArtery);
}
