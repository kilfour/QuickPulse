using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Emits the given objects into the current artery. Use to record static traces or messages.
    /// </summary>
    public static Flow<Unit> Trace(params object[] data) =>
        Runnel(Always, _ => data, IntoArtery);

    /// <summary>
    /// Emits a value extracted from the current state into the artery. Use for dynamic or computed traces.
    /// </summary>
    public static Flow<Unit> Trace<T>(Func<T, object> extractor) =>
        Runnel(Always, ExtractDataFromBox(extractor), IntoArtery);
}
