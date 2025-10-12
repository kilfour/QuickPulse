using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> TraceIf(bool flag, Func<object> data) =>
        Runnel(Flag(flag), _ => data(), IntoArtery);
    public static Flow<Unit> TraceIf<T>(Func<T, bool> predicate, Func<object> data) =>
        Runnel(Sluice(predicate), _ => data(), IntoArtery);
    public static Flow<Unit> TraceIf<T>(Func<T, bool> predicate, Func<T, object> extractor) =>
        Runnel(Sluice(predicate), ExtractDataFromBox(extractor), IntoArtery);
}
