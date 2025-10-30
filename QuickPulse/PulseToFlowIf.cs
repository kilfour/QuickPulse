namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Executes the subflow with a single value when the flag is true. 
    /// Use for optional flow invocation.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(bool flag, Flow<TValue> flow, Func<TValue> value) =>
        Emit(Flag(flag), Single(value), IntoFlow(flow));

    /// <summary>
    /// Executes the subflow for each value in the collection when the flag is true. 
    /// Use to conditionally process batches.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(bool flag, Flow<TValue> flow, Func<IEnumerable<TValue>> values) =>
        Emit(Flag(flag), Many(values), IntoFlow(flow));

    /// <summary>
    /// Executes a factory-generated subflow with a single value when the flag is true. 
    /// Use for deferred or dynamic flow creation.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(bool flag, Func<TValue, Flow<Flow>> flowFactory, Func<TValue> value) =>
        Emit(Flag(flag), Single(value), IntoFactory(flowFactory));

    /// <summary>
    /// Executes a factory-generated subflow for each value in the collection when the flag is true. 
    /// Use for conditional dynamic fan-out.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(bool flag, Func<TValue, Flow<Flow>> flowFactory, Func<IEnumerable<TValue>> values) =>
        Emit(Flag(flag), Many(values), IntoFactory(flowFactory));

    /// <summary>
    /// Executes the subflow with a single value when the predicate is true for the current state.
    /// Use for state-aware conditional invocation.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(Func<TCell, bool> predicate, Flow<TValue> flow, Func<TValue> value) =>
        Emit(Gate(predicate), Single(value), IntoFlow(flow));

    /// <summary>
    /// Executes the subflow for each value in the collection when the predicate is true for the current state. 
    /// Use for context-sensitive batch processing.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(Func<TCell, bool> predicate, Flow<TValue> flow, Func<IEnumerable<TValue>> values) =>
        Emit(Gate(predicate), Many(values), IntoFlow(flow));

    /// <summary>
    /// Executes a factory-generated subflow with a single value when the predicate is true for the current state. 
    /// Use for state-aware conditional invocation.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(Func<TCell, bool> predicate, Func<TValue, Flow<Flow>> flowFactory, Func<TValue> value) =>
        Emit(Gate(predicate), Single(value), IntoFactory(flowFactory));

    /// <summary>
    /// Executes a factory-generated subflow for each value in the collection when the predicate is true for the current state. 
    /// Use for context-sensitive batch processing.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(Func<TCell, bool> predicate, Func<TValue, Flow<Flow>> flowFactory, Func<IEnumerable<TValue>> values) =>
        Emit(Gate(predicate), Many(values), IntoFactory(flowFactory));
}
