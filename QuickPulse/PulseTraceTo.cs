using QuickPulse.Arteries;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Emits the given objects into the specified grafted artery. 
    /// Use to direct traces to a custom or secondary output channel.
    /// </summary>
    public static Flow<Flow> TraceTo<TArtery>(params object[] data) where TArtery : IArtery =>
        Emit(Always, _ => data, IntoGraftedArtery<TArtery>());

    /// <summary>
    /// Emits the given objects into the specified grafted artery. 
    /// Use to direct traces to a custom or secondary output channel.
    /// </summary>
    public static Flow<Flow> TraceTo<TArtery>(this Flow<Flow> other, params object[] data) where TArtery : IArtery =>
        other.Then(TraceTo<TArtery>(data));

    /// <summary>
    /// Emits a formatted representation of the value currently carried by the flow
    /// into the specified artery, then continues as a continuation flow.
    /// </summary>
    public static Flow<Flow> TraceTo<TArtery, T>(this Flow<T> other, Func<T, object> formatter) where TArtery : IArtery =>
        other.SelectMany(a => TraceTo<TArtery>(formatter(a)));

    /// <summary>
    /// Emits the value currently carried by the flow into the specified artery.
    /// Use to trace the carried value unchanged to a custom or secondary output channel.
    /// </summary>
    public static Flow<Flow> TraceTo<TArtery, T>(this Flow<T> other) where TArtery : IArtery =>
        other.SelectMany(a => TraceTo<TArtery>(a!));
}
