namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Stops the current flow from accepting further pulses. Use to end execution explicitly.
    /// </summary>
    public static Flow<Flow> StopFlowing() => GiveMeABeat(s => s.StopFlowing());

    /// <summary>
    /// Stops the current flow from accepting further pulses. Use to end execution explicitly.
    /// </summary>
    public static Flow<Flow> StopFlowing(this Flow<Flow> other) =>
        other.Then(StopFlowing());
}
