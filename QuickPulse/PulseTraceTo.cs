using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State, object> IntoGraftedArtery<TArtery>() where TArtery : IArtery =>
        (s, o) => s.GetArtery<TArtery>().Absorb(o);

    public static Flow<Unit> TraceTo<TArtery>(params object[] data) where TArtery : IArtery =>
        Runnel(Always, _ => data, IntoGraftedArtery<TArtery>());
    public static Flow<Unit> TraceToIf<TArtery>(bool flag, Func<object> data) where TArtery : IArtery =>
        Runnel(Flag(flag), _ => data(), IntoGraftedArtery<TArtery>());
    public static Flow<Unit> TraceToIf<TArtery, T>(Func<T, bool> predicate, Func<object> data) where TArtery : IArtery =>
        Runnel(Sluice(predicate), _ => data(), IntoGraftedArtery<TArtery>());
    public static Flow<Unit> TraceToIf<TArtery, T>(Func<T, bool> predicate, Func<T, object> data) where TArtery : IArtery =>
        Runnel(Sluice(predicate), s => { var v = s.GetTheBox<T>().Value; return data(v); }, IntoGraftedArtery<TArtery>());
}
