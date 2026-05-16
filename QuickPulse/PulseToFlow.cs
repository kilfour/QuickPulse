namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Executes the given subflow once with the specified value. 
    /// Use to invoke a reusable flow inline.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(Flow<TValue> flow, TValue value) =>
        Emit(Always, Single(value), IntoFlow(flow));

    /// <summary>
    /// Executes the given subflow once with the specified value. 
    /// Use to invoke a reusable flow inline.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(this Flow<Flow> other, Flow<TValue> flow, TValue value) =>
        other.Then(ToFlow(flow, value));

    /// <summary>
    /// Executes the given subflow once with a value derived from the current flow result.
    /// Use to pass the carried value into a reusable subflow inline.
    /// </summary>
    public static Flow<Flow> ToFlow<TSource, TValue>(this Flow<TSource> other, Flow<TValue> flow, Func<TSource, TValue> value) =>
        other.SelectMany(a => ToFlow(flow, value(a)));

    /// <summary>
    /// Executes the given subflow for each value in the collection. 
    /// Use to fan out work over multiple inputs.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(Flow<TValue> flow, IEnumerable<TValue> values) =>
        Emit(Always, Many(values), IntoFlow(flow));

    /// <summary>
    /// Executes the given subflow for each value in the collection. 
    /// Use to fan out work over multiple inputs.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(this Flow<Flow> other, Flow<TValue> flow, IEnumerable<TValue> values) =>
        other.Then(ToFlow(flow, values));

    /// <summary>
    /// Executes the given subflow for each value derived from the current flow result.
    /// Use to fan out work from the carried value.
    /// </summary>
    public static Flow<Flow> ToFlow<TSource, TValue>(this Flow<TSource> other, Flow<TValue> flow, Func<TSource, IEnumerable<TValue>> values) =>
        other.SelectMany(a => ToFlow(flow, values(a)));

    /// <summary>
    /// Executes a subflow produced by the given factory once with the specified value. 
    /// Use for dynamic subflow creation.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(Func<TValue, Flow<Flow>> flowFactory, TValue value) =>
        Emit(Always, Single(value), IntoFactory(flowFactory));

    /// <summary>
    /// Executes a subflow produced by the given factory once with the specified value. 
    /// Use for dynamic subflow creation.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(this Flow<Flow> other, Func<TValue, Flow<Flow>> flowFactory, TValue value) =>
        other.Then(ToFlow(flowFactory, value));

    /// <summary>
    /// Executes a subflow produced from a value derived from the current flow result.
    /// Use for dynamic subflow creation based on the carried value.
    /// </summary>
    public static Flow<Flow> ToFlow<TSource, TValue>(this Flow<TSource> other, Func<TValue, Flow<Flow>> flowFactory, Func<TSource, TValue> value) =>
        other.SelectMany(a => ToFlow(flowFactory, value(a)));

    /// <summary>
    /// Executes a subflow produced by the given factory for each value in the collection. 
    /// Use for dynamically generated fan-out flows.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(Func<TValue, Flow<Flow>> flowFactory, IEnumerable<TValue> values) =>
        Emit(Always, Many(values), IntoFactory(flowFactory));

    /// <summary>
    /// Executes a subflow produced by the given factory for each value in the collection. 
    /// Use for dynamically generated fan-out flows.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(this Flow<Flow> other, Func<TValue, Flow<Flow>> flowFactory, IEnumerable<TValue> values) =>
        other.Then(ToFlow(flowFactory, values));

    /// <summary>
    /// Executes subflows produced from values derived from the current flow result.
    /// Use for dynamically generated fan-out based on the carried value.
    /// </summary>
    public static Flow<Flow> ToFlow<TSource, TValue>(this Flow<TSource> other, Func<TValue, Flow<Flow>> flowFactory, Func<TSource, IEnumerable<TValue>> values) =>
        other.SelectMany(a => ToFlow(flowFactory, values(a)));
}
