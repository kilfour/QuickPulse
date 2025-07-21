using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<TOut> Start<TOut>() =>
        state => Cask.Some(state, state.GetValue<TOut>());
}
