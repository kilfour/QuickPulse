namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Executes a subflow for each input and discards its result. Use to process or trace values without emitting new ones.
    /// </summary>
    public static Flow<Flow> Drain<T>(Func<T, Flow<Flow>> step)
        => Drain<T, Flow>(step);

    /// <summary>
    /// Executes a subflow returning any inner type and discards both its result and the original input. Use for one-way, sink-like operations.
    /// </summary>
    public static Flow<Flow> Drain<T, TInner>(Func<T, Flow<TInner>> step)
        => Start(step).Dissipate();
}
