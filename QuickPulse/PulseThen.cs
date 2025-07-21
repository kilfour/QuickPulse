using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<TResult> Then<TSource, TResult>(this Flow<TSource> flow, Flow<TResult> next)
    {
        return flow.SelectMany(_ => next);
    }
}