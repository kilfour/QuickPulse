using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static readonly Action<State, object> IntoArtery =
        (s, o) => s.CurrentArtery!.Absorb(o);
    private static Func<State, object> ExtractDataFromBox<T>(Func<T, object> data) =>
        s => { var v = s.GetTheBox<T>().Value; return data(v); };

    public static Flow<Unit> Trace(params object[] data) =>
        Runnel(Always, _ => data, IntoArtery);
    public static Flow<Unit> Trace<T>(Func<T, object> extractor) =>
        Runnel(Always, ExtractDataFromBox(extractor), IntoArtery);
}
