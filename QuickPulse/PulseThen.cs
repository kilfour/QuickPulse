namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<TResult> Then<TSource, TResult>(this Flow<TSource> flow, Flow<TResult> next)
        => flow.SelectMany(_ => next);
}