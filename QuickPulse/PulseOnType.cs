namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Executes the subflow if the given object is of type TDerived. Use for safely routing mixed inputs to typed flows.
    /// </summary>
    public static Flow<Flow> OnType<TBase, TDerived>(Flow<TDerived> flow, Func<TBase> value)
        where TDerived : TBase =>
        Emit(Flag(value() is TDerived), Single(() => (TDerived)value()!), IntoFlow(flow));

    /// <summary>
    /// Executes the subflow if the given object is of type TDerived. Use for safely routing mixed inputs to typed flows.
    /// </summary>
    public static Flow<Flow> OnType<TBase, TDerived>(this Flow<Flow> other, Flow<TDerived> flow, Func<TBase> value)
        where TDerived : TBase =>
        other.Then(OnType(flow, value));

    /// <summary>
    /// Executes the subflow if the current flow result is of type TDerived.
    /// Use to route a carried value to a typed subflow without re-wrapping it in a lambda.
    /// </summary>
    public static Flow<Flow> OnType<TBase, TDerived>(this Flow<TBase> other, Flow<TDerived> flow)
        where TDerived : TBase =>
        other.SelectMany(a => a is TDerived derived ? ToFlow(flow, derived) : NoOp());

    /// <summary>
    /// Executes the subflow produced by the given factory if the given object is of type TDerived. 
    /// Use for safely routing mixed inputs to typed flows.
    /// </summary>
    public static Flow<Flow> OnType<TBase, TDerived>(Func<TDerived, Flow<Flow>> flowFactory, Func<TBase> value)
        where TDerived : TBase =>
        Emit(Flag(value() is TDerived), Single(() => (TDerived)value()!), IntoFactory(flowFactory));

    /// <summary>
    /// Executes the subflow produced by the given factory if the given object is of type TDerived. 
    /// Use for safely routing mixed inputs to typed flows.
    /// </summary>
    public static Flow<Flow> OnType<TBase, TDerived>(this Flow<Flow> other, Func<TDerived, Flow<Flow>> flowFactory, Func<TBase> value)
        where TDerived : TBase =>
        other.Then(OnType(flowFactory, value));

    /// <summary>
    /// Executes the subflow produced by the given factory if the current flow result is of type TDerived.
    /// Use to branch on the carried value's runtime type.
    /// </summary>
    public static Flow<Flow> OnType<TBase, TDerived>(this Flow<TBase> other, Func<TDerived, Flow<Flow>> flowFactory)
        where TDerived : TBase =>
        other.SelectMany(a => a is TDerived derived ? flowFactory(derived) : NoOp());

}
