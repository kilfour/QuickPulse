namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Runs the next flow after the current one completes, preserving shared state. 
    /// Use to sequence flows declaratively.
    /// </summary>
    public static Flow<TResult> ContinueWith<TSource, TResult>(this Flow<TSource> flow, Func<TSource, Flow<TResult>> next)
        => flow.SelectMany(next);
}
