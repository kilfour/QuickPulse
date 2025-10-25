namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Executes the given flow when the flag is true. Use for simple conditional execution inside a flow.
    /// </summary>
    public static Flow<Flow> When(bool flag, Flow<Flow> flow) =>
        Runnel(Flag(flag), s => flow(s));

    /// <summary>
    /// Executes the given flow when the predicate evaluates to true for the current state. Use for state-aware conditional branching.
    /// </summary>
    public static Flow<Flow> When<T>(Func<T, bool> predicate, Flow<Flow> flow) =>
        Runnel(Sluice(predicate), s => flow(s));

    /// <summary>
    /// Executes the flow created by the factory when the flag is true. Use to defer flow creation until needed.
    /// </summary>
    public static Flow<Flow> When(bool flag, Func<Flow<Flow>> flowFactory) =>
        Runnel(Flag(flag), s => flowFactory()(s));

    /// <summary>
    /// Executes the flow created by the factory when the predicate is true for the current state. Use for lazy, context-sensitive branching.
    /// </summary>
    public static Flow<Flow> When<T>(Func<T, bool> predicate, Func<Flow<Flow>> flowFactory) =>
        Runnel(Sluice(predicate), s => flowFactory()(s));
}
