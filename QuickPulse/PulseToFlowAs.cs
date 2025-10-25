namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Executes the subflow if the given object is of type T. Use for safely routing mixed inputs to typed flows.
    /// </summary>
    public static Flow<Flow> ToFlowAs<T>(Flow<T> flow, object value) where T : class =>
        Runnel(Flag(value is T v), Single(value as T)!, IntoFlow(flow)!);

    /// <summary>
    /// Executes a subflow from the factory if the given object is of type T. Use for conditional dynamic subflows.
    /// </summary>
    public static Flow<Flow> ToFlowAs<T>(Func<T, Flow<Flow>> flowFactory, object value) where T : class =>
        Runnel(Flag(value is T v), Single(value as T)!, IntoFactory(flowFactory)!);

    /// <summary>
    /// Executes the subflow for each object in the collection that matches type T. Use to filter and process heterogeneous inputs.
    /// </summary>
    public static Flow<Flow> ToFlowAs<T>(Flow<T> flow, IEnumerable<object> values) where T : class =>
        Runnel(Always, Many(values.OfType<T>()), IntoFlow(flow));

    /// <summary>
    /// Executes a factory-generated subflow for each object matching type T. Use for dynamic handling of mixed-type collections.
    /// </summary>
    public static Flow<Flow> ToFlowAs<T>(Func<T, Flow<Flow>> flowFactory, IEnumerable<object> values) where T : class =>
        Runnel(Always, Many(values.OfType<T>()), IntoFactory(flowFactory));
}
