using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static readonly Action<State, object> IntoArtery =
        (s, o) => s.CurrentArtery?.Flow(o);
    public static Flow<Unit> Trace(params object[] data) =>
        Runnel(Always, _ => data, IntoArtery);
    public static Flow<Unit> TraceIf(bool flag, Func<object> data) =>
        Runnel(Flag(flag), _ => new[] { data() }, IntoArtery);
    public static Flow<Unit> TraceIf<T>(Func<T, bool> predicate, Func<object> data) =>
        Runnel(Sluice(predicate), _ => new[] { data() }, IntoArtery);
}
