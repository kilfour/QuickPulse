using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<TOut> Start<TOut>() =>
        s => Cask.Some(s, s.GetValue<TOut>());
}
