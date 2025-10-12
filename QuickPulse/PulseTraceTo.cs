using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State, object> IntoGraftedArtery<TArtery>() where TArtery : IArtery =>
        (s, o) => s.GetArtery<TArtery>().Absorb(o);

    /// <summary>
    /// Emits the given objects into the specified grafted artery. Use to direct traces to a custom or secondary output channel.
    /// </summary>
    public static Flow<Unit> TraceTo<TArtery>(params object[] data) where TArtery : IArtery =>
        Runnel(Always, _ => data, IntoGraftedArtery<TArtery>());

    /// <summary>
    /// Emits a trace into the specified artery when the flag is true. Use for conditional output to auxiliary channels.
    /// </summary>
    public static Flow<Unit> TraceToIf<TArtery>(bool flag, Func<object> data) where TArtery : IArtery =>
        Runnel(Flag(flag), _ => data(), IntoGraftedArtery<TArtery>());

    /// <summary>
    /// Emits a trace into the specified artery when the predicate is true for the current state. Use for state-driven conditional tracing.
    /// </summary>
    public static Flow<Unit> TraceToIf<TArtery, T>(Func<T, bool> predicate, Func<object> data) where TArtery : IArtery =>
        Runnel(Sluice(predicate), _ => data(), IntoGraftedArtery<TArtery>());

    /// <summary>
    /// Emits a computed trace into the specified artery when the predicate is true for the current state. Use for context-sensitive diagnostics on alternate channels.
    /// </summary>
    public static Flow<Unit> TraceToIf<TArtery, T>(Func<T, bool> predicate, Func<T, object> data) where TArtery : IArtery =>
        Runnel(Sluice(predicate), s => { var v = s.GetTheBox<T>().Value; return data(v); }, IntoGraftedArtery<TArtery>());
}
