using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> Into<T>(Flow<T> flow, object value) where T : class =>
        Runnel(Flag(value is T v), Single(value as T)!, IntoFlow(flow)!);
    public static Flow<Unit> Into<T>(Func<T, Flow<Unit>> flowFactory, object value) where T : class =>
        Runnel(Flag(value is T v), Single(value as T)!, IntoFactory(flowFactory)!);
    public static Flow<Unit> Into<T>(Flow<T> flow, IEnumerable<object> values) where T : class =>
        Runnel(Always, Many(values.OfType<T>()), IntoFlow(flow));
    public static Flow<Unit> Into<T>(Func<T, Flow<Unit>> flowFactory, IEnumerable<object> values) where T : class =>
        Runnel(Always, Many(values.OfType<T>()), IntoFactory(flowFactory));

}