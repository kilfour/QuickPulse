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
    /// Executes the subflow produced by the given factory if the given object is of type TDerived. 
    /// Use for safely routing mixed inputs to typed flows.
    /// </summary>
    public static Flow<Flow> OnType<TBase, TDerived>(Func<TDerived, Flow<Flow>> flowFactory, Func<TBase> value)
        where TDerived : TBase =>
        Emit(Flag(value() is TDerived), Single(() => (TDerived)value()!), IntoFactory(flowFactory));

}
