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
    /// Executes the given subflow for each value in the collection. 
    /// Use to fan out work over multiple inputs.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(Flow<TValue> flow, IEnumerable<TValue> values) =>
        Emit(Always, Many(values), IntoFlow(flow));

    /// <summary>
    /// Executes a subflow produced by the given factory once with the specified value. 
    /// Use for dynamic subflow creation.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(Func<TValue, Flow<Flow>> flowFactory, TValue value) =>
        Emit(Always, Single(value), IntoFactory(flowFactory));

    /// <summary>
    /// Executes a subflow produced by the given factory for each value in the collection. 
    /// Use for dynamically generated fan-out flows.
    /// </summary>
    public static Flow<Flow> ToFlow<TValue>(Func<TValue, Flow<Flow>> flowFactory, IEnumerable<TValue> values) =>
        Emit(Always, Many(values), IntoFactory(flowFactory));
}
