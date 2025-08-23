using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    // --------------------------------------------------------------------------------
    // -- Adapters 
    // --
    private static Action<State, T> IntoFlow<T>(Flow<T> flow)
        => (s, v) => flow(s.SetValue(v));
    private static Action<State, T> IntoFactory<T>(Func<T, Flow<Unit>> flowFactory)
        => (s, v) => GetFlowFromFactory(flowFactory)(s.SetValue(v));
    // --------------------------------------------------------------------------------
    // -- Non Conditional 
    // --
    public static Flow<Unit> ToFlow<T>(Flow<T> flow, T value) =>
        Runnel(Always, Single(value), IntoFlow(flow));
    public static Flow<Unit> ToFlow<T>(Flow<T> flow, IEnumerable<T> values) =>
        Runnel(Always, Many(values), IntoFlow(flow));
    public static Flow<Unit> ToFlow<T>(Func<T, Flow<Unit>> flowFactory, T value) =>
        Runnel(Always, Single(value), IntoFactory(flowFactory));
    public static Flow<Unit> ToFlow<T>(Func<T, Flow<Unit>> flowFactory, IEnumerable<T> values) =>
        Runnel(Always, Many(values), IntoFactory(flowFactory));
    // --------------------------------------------------------------------------------
    // -- Conditional 
    // --
    public static Flow<Unit> ToFlowIf<T>(bool flag, Flow<T> flow, Func<T> value) =>
        Runnel(Flag(flag), Single(value), IntoFlow(flow));
    public static Flow<Unit> ToFlowIf<T>(bool flag, Flow<T> flow, Func<IEnumerable<T>> values) =>
        Runnel(Flag(flag), Many(values), IntoFlow(flow));
    public static Flow<Unit> ToFlowIf<T>(bool flag, Func<T, Flow<Unit>> flowFactory, Func<T> value) =>
        Runnel(Flag(flag), Single(value), IntoFactory(flowFactory));
    public static Flow<Unit> ToFlowIf<T>(bool flag, Func<T, Flow<Unit>> flowFactory, Func<IEnumerable<T>> values) =>
        Runnel(Flag(flag), Many(values), IntoFactory(flowFactory));
    // --------------------------------------------------------------------------------
    // -- On Type
    // --
    public static Flow<Unit> ToFlow<T>(Flow<T> flow, object value) where T : class =>
        Runnel(Flag(value is T v), Single(value as T)!, IntoFlow(flow)!);
    public static Flow<Unit> ToFlow<T>(Func<T, Flow<Unit>> flowFactory, object value) where T : class =>
        Runnel(Flag(value is T v), Single(value as T)!, IntoFactory(flowFactory)!);
    public static Flow<Unit> ToFlow<T>(Flow<T> flow, IEnumerable<object> values) where T : class =>
        Runnel(Always, Many(values.OfType<T>()), IntoFlow(flow));
    public static Flow<Unit> ToFlow<T>(Func<T, Flow<Unit>> flowFactory, IEnumerable<object> values) where T : class =>
        Runnel(Always, Many(values.OfType<T>()), IntoFactory(flowFactory));

    // --------------------------------------------------------------------------------
    // -- Boxed Conditional 
    // --
    public static Flow<Unit> ToFlowIf<T, TBox>(Func<TBox, bool> predicate, Flow<T> flow, Func<T> value) =>
        Runnel(Sluice(predicate), Single(value), IntoFlow(flow));
    public static Flow<Unit> ToFlowIf<T, TBox>(Func<TBox, bool> predicate, Flow<T> flow, Func<IEnumerable<T>> values) =>
        Runnel(Sluice(predicate), Many(values), IntoFlow(flow));
}