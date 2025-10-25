namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Runs the flow and discards its output. Use when the result is unneeded but the side effects matter.
    /// </summary>
    public static Flow<Flow> Dissipate<T>(this Flow<T> flow)
        => flow.Then(NoOp());
}
