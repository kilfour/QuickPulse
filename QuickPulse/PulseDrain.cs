namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Executes a subflow for each input and discards its result. Use to process or trace values without emitting new ones.
    /// </summary>
    public static Flow<Unit> Drain<T>(Func<T, Flow<Unit>> step)
        => Drain<T, Unit>(step);

    /// <summary>
    /// Executes a subflow returning any inner type and discards both its result and the original input. Use for one-way, sink-like operations.
    /// </summary>
    public static Flow<Unit> Drain<T, TInner>(Func<T, Flow<TInner>> step)
        => Circulate(step).Dissipate();
}
