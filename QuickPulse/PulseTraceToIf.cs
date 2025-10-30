using QuickPulse.Arteries;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Emits a trace into the specified artery when the flag is true. 
    /// Use for conditional output to auxiliary channels.
    /// </summary>
    public static Flow<Flow> TraceToIf<TArtery>(bool flag, Func<object> data) where TArtery : IArtery =>
        Emit(Flag(flag), _ => data(), IntoGraftedArtery<TArtery>());

    /// <summary>
    /// Emits a trace into the specified artery when the predicate is true for the current state. 
    /// Use for state-driven conditional tracing.
    /// </summary>
    public static Flow<Flow> TraceToIf<TArtery, TCell>(Func<TCell, bool> predicate, Func<object> data) where TArtery : IArtery =>
        Emit(Gate(predicate), _ => data(), IntoGraftedArtery<TArtery>());

    /// <summary>
    /// Emits a computed trace into the specified artery when the predicate is true for the current state. 
    /// Use for context-sensitive diagnostics on alternate channels.
    /// </summary>
    public static Flow<Flow> TraceToIf<TArtery, TCell>(Func<TCell, bool> predicate, Func<TCell, object> data) where TArtery : IArtery =>
        Emit(Gate(predicate), s => { var v = s.GetTheCell<TCell>().Value; return data(v); }, IntoGraftedArtery<TArtery>());
}
