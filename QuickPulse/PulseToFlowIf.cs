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
    /// Executes the subflow with a single value when the flag is true after the current flow completes.
    /// Use for optional inline flow invocation in a builder chain.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(this Flow<Flow> other, bool flag, Flow<TValue> flow, Func<TValue> value) =>
        other.Then(ToFlowIf(flag, flow, value));

    /// <summary>
    /// Executes the subflow with a value derived from the current flow result when the flag is true.
    /// Use to conditionally feed the carried value into a subflow.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TSource, TValue>(this Flow<TSource> other, bool flag, Flow<TValue> flow, Func<TSource, TValue> value) =>
        other.SelectMany(a => ToFlowIf(flag, flow, () => value(a)));

    /// <summary>
    /// Executes the subflow for each value in the collection when the flag is true. 
    /// Use to conditionally process batches.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(bool flag, Flow<TValue> flow, Func<IEnumerable<TValue>> values) =>
        Emit(Flag(flag), Many(values), IntoFlow(flow));

    /// <summary>
    /// Executes the subflow for each value in the collection when the flag is true after the current flow completes.
    /// Use to conditionally process batches in a builder chain.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(this Flow<Flow> other, bool flag, Flow<TValue> flow, Func<IEnumerable<TValue>> values) =>
        other.Then(ToFlowIf(flag, flow, values));

    /// <summary>
    /// Executes the subflow for each value derived from the current flow result when the flag is true.
    /// Use for conditional fan-out from the carried value.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TSource, TValue>(this Flow<TSource> other, bool flag, Flow<TValue> flow, Func<TSource, IEnumerable<TValue>> values) =>
        other.SelectMany(a => ToFlowIf(flag, flow, () => values(a)));

    /// <summary>
    /// Executes a factory-generated subflow with a single value when the flag is true. 
    /// Use for deferred or dynamic flow creation.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(bool flag, Func<TValue, Flow<Flow>> flowFactory, Func<TValue> value) =>
        Emit(Flag(flag), Single(value), IntoFactory(flowFactory));

    /// <summary>
    /// Executes a factory-generated subflow with a single value when the flag is true after the current flow completes.
    /// Use for optional deferred flow creation in a builder chain.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(this Flow<Flow> other, bool flag, Func<TValue, Flow<Flow>> flowFactory, Func<TValue> value) =>
        other.Then(ToFlowIf(flag, flowFactory, value));

    /// <summary>
    /// Executes a factory-generated subflow with a value derived from the current flow result when the flag is true.
    /// Use for conditional dynamic flow creation from the carried value.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TSource, TValue>(this Flow<TSource> other, bool flag, Func<TValue, Flow<Flow>> flowFactory, Func<TSource, TValue> value) =>
        other.SelectMany(a => ToFlowIf(flag, flowFactory, () => value(a)));

    /// <summary>
    /// Executes a factory-generated subflow for each value in the collection when the flag is true. 
    /// Use for conditional dynamic fan-out.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(bool flag, Func<TValue, Flow<Flow>> flowFactory, Func<IEnumerable<TValue>> values) =>
        Emit(Flag(flag), Many(values), IntoFactory(flowFactory));

    /// <summary>
    /// Executes a factory-generated subflow for each value in the collection when the flag is true after the current flow completes.
    /// Use for conditional dynamic fan-out in a builder chain.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue>(this Flow<Flow> other, bool flag, Func<TValue, Flow<Flow>> flowFactory, Func<IEnumerable<TValue>> values) =>
        other.Then(ToFlowIf(flag, flowFactory, values));

    /// <summary>
    /// Executes factory-generated subflows for values derived from the current flow result when the flag is true.
    /// Use for conditional dynamic fan-out from the carried value.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TSource, TValue>(this Flow<TSource> other, bool flag, Func<TValue, Flow<Flow>> flowFactory, Func<TSource, IEnumerable<TValue>> values) =>
        other.SelectMany(a => ToFlowIf(flag, flowFactory, () => values(a)));

    /// <summary>
    /// Executes the subflow with a single value when the predicate is true for the current state.
    /// Use for state-aware conditional invocation.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(Func<TCell, bool> predicate, Flow<TValue> flow, Func<TValue> value) =>
        Emit(Gate(predicate), Single(value), IntoFlow(flow));

    /// <summary>
    /// Executes the subflow with a single value when the predicate is true for the current state after the current flow completes.
    /// Use for state-aware conditional chaining.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(this Flow<Flow> other, Func<TCell, bool> predicate, Flow<TValue> flow, Func<TValue> value) =>
        other.Then(ToFlowIf(predicate, flow, value));

    /// <summary>
    /// Executes the subflow with a value derived from the current flow result when the predicate is true for that value.
    /// Use for carried-value conditional invocation.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TSource, TValue>(this Flow<TSource> other, Func<TSource, bool> predicate, Flow<TValue> flow, Func<TSource, TValue> value) =>
        other.SelectMany(a => ToFlowIf(predicate(a), flow, () => value(a)));

    /// <summary>
    /// Executes the subflow for each value in the collection when the predicate is true for the current state. 
    /// Use for context-sensitive batch processing.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(Func<TCell, bool> predicate, Flow<TValue> flow, Func<IEnumerable<TValue>> values) =>
        Emit(Gate(predicate), Many(values), IntoFlow(flow));

    /// <summary>
    /// Executes the subflow for each value in the collection when the predicate is true for the current state after the current flow completes.
    /// Use for state-aware batch chaining.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(this Flow<Flow> other, Func<TCell, bool> predicate, Flow<TValue> flow, Func<IEnumerable<TValue>> values) =>
        other.Then(ToFlowIf(predicate, flow, values));

    /// <summary>
    /// Executes the subflow for each value derived from the current flow result when the predicate is true for that value.
    /// Use for conditional fan-out from the carried value.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TSource, TValue>(this Flow<TSource> other, Func<TSource, bool> predicate, Flow<TValue> flow, Func<TSource, IEnumerable<TValue>> values) =>
        other.SelectMany(a => ToFlowIf(predicate(a), flow, () => values(a)));

    /// <summary>
    /// Executes a factory-generated subflow with a single value when the predicate is true for the current state. 
    /// Use for state-aware conditional invocation.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(Func<TCell, bool> predicate, Func<TValue, Flow<Flow>> flowFactory, Func<TValue> value) =>
        Emit(Gate(predicate), Single(value), IntoFactory(flowFactory));

    /// <summary>
    /// Executes a factory-generated subflow with a single value when the predicate is true for the current state after the current flow completes.
    /// Use for state-aware deferred chaining.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(this Flow<Flow> other, Func<TCell, bool> predicate, Func<TValue, Flow<Flow>> flowFactory, Func<TValue> value) =>
        other.Then(ToFlowIf(predicate, flowFactory, value));

    /// <summary>
    /// Executes a factory-generated subflow with a value derived from the current flow result when the predicate is true for that value.
    /// Use for conditional dynamic flow creation from the carried value.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TSource, TValue>(this Flow<TSource> other, Func<TSource, bool> predicate, Func<TValue, Flow<Flow>> flowFactory, Func<TSource, TValue> value) =>
        other.SelectMany(a => ToFlowIf(predicate(a), flowFactory, () => value(a)));

    /// <summary>
    /// Executes a factory-generated subflow for each value in the collection when the predicate is true for the current state. 
    /// Use for context-sensitive batch processing.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(Func<TCell, bool> predicate, Func<TValue, Flow<Flow>> flowFactory, Func<IEnumerable<TValue>> values) =>
        Emit(Gate(predicate), Many(values), IntoFactory(flowFactory));

    /// <summary>
    /// Executes factory-generated subflows for each value in the collection when the predicate is true for the current state after the current flow completes.
    /// Use for state-aware dynamic batch chaining.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TValue, TCell>(this Flow<Flow> other, Func<TCell, bool> predicate, Func<TValue, Flow<Flow>> flowFactory, Func<IEnumerable<TValue>> values) =>
        other.Then(ToFlowIf(predicate, flowFactory, values));

    /// <summary>
    /// Executes factory-generated subflows for values derived from the current flow result when the predicate is true for that value.
    /// Use for conditional dynamic fan-out from the carried value.
    /// </summary>
    public static Flow<Flow> ToFlowIf<TSource, TValue>(this Flow<TSource> other, Func<TSource, bool> predicate, Func<TValue, Flow<Flow>> flowFactory, Func<TSource, IEnumerable<TValue>> values) =>
        other.SelectMany(a => ToFlowIf(predicate(a), flowFactory, () => values(a)));
}
