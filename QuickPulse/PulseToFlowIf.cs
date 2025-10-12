namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Executes the subflow with a single value when the flag is true. Use for optional flow invocation.
    /// </summary>
    public static Flow<Unit> ToFlowIf<T>(bool flag, Flow<T> flow, Func<T> value) =>
        Runnel(Flag(flag), Single(value), IntoFlow(flow));

    /// <summary>
    /// Executes the subflow for each value in the collection when the flag is true. Use to conditionally process batches.
    /// </summary>
    public static Flow<Unit> ToFlowIf<T>(bool flag, Flow<T> flow, Func<IEnumerable<T>> values) =>
        Runnel(Flag(flag), Many(values), IntoFlow(flow));

    /// <summary>
    /// Executes a factory-generated subflow with a single value when the flag is true. Use for deferred or dynamic flow creation.
    /// </summary>
    public static Flow<Unit> ToFlowIf<T>(bool flag, Func<T, Flow<Unit>> flowFactory, Func<T> value) =>
        Runnel(Flag(flag), Single(value), IntoFactory(flowFactory));

    /// <summary>
    /// Executes a factory-generated subflow for each value in the collection when the flag is true. Use for conditional dynamic fan-out.
    /// </summary>
    public static Flow<Unit> ToFlowIf<T>(bool flag, Func<T, Flow<Unit>> flowFactory, Func<IEnumerable<T>> values) =>
        Runnel(Flag(flag), Many(values), IntoFactory(flowFactory));

    /// <summary>
    /// Executes the subflow with a single value when the predicate is true for the current state. Use for state-aware conditional invocation.
    /// </summary>
    public static Flow<Unit> ToFlowIf<T, TBox>(Func<TBox, bool> predicate, Flow<T> flow, Func<T> value) =>
        Runnel(Sluice(predicate), Single(value), IntoFlow(flow));

    /// <summary>
    /// Executes the subflow for each value in the collection when the predicate is true for the current state. Use for context-sensitive batch processing.
    /// </summary>
    public static Flow<Unit> ToFlowIf<T, TBox>(Func<TBox, bool> predicate, Flow<T> flow, Func<IEnumerable<T>> values) =>
        Runnel(Sluice(predicate), Many(values), IntoFlow(flow));
}
