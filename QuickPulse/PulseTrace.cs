using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static readonly Action<State, object> IntoArtery =
        (s, o) => s.CurrentArtery!.Flow(o);
    private static Func<State, object> ExtractDataFromBox<T>(Func<T, object> data) =>
        s => { var v = s.GetTheBox<T>().Value; return data(v); };

    public static Flow<Unit> Trace(params object[] data) =>
        Runnel(Always, _ => data, IntoArtery);
    public static Flow<Unit> Trace<T>(Func<T, object> extractor) =>
        Runnel(Always, ExtractDataFromBox(extractor), IntoArtery);

    public static Flow<Unit> TraceIf(bool flag, Func<object> data) =>
        Runnel(Flag(flag), _ => data(), IntoArtery);
    public static Flow<Unit> TraceIf<T>(Func<T, bool> predicate, Func<object> data) =>
        Runnel(Sluice(predicate), _ => data(), IntoArtery);
    public static Flow<Unit> TraceIf<T>(Func<T, bool> predicate, Func<T, object> extractor) =>
        Runnel(Sluice(predicate), ExtractDataFromBox(extractor), IntoArtery);


}
