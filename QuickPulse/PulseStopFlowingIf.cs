namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Stops the current flow when the condition is true. Use for simple termination guards.
    /// </summary>
    public static Flow<Unit> StopFlowingIf(bool condition) =>
        condition ? StopFlowing() : NoOp();

    /// <summary>
    /// Stops the current flow when the predicate evaluates to true for the current state. Use for data-driven termination.
    /// </summary>
    public static Flow<Unit> StopFlowingIf<T>(Func<T, bool> predicate) =>
        When(predicate, StopFlowing());
}
