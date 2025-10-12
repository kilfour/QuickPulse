namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Runs the next flow after the current one completes, preserving shared state. Use to sequence flows declaratively.
    /// </summary>
    public static Flow<TResult> Then<TSource, TResult>(this Flow<TSource> flow, Flow<TResult> next)
        => flow.SelectMany(_ => next);
}
