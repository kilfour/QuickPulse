using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<TOut> Start<TOut>() =>
        state => Cask.Some(state, state.GetValue<TOut>());

    // public static Flow<Unit> Using(IArtery artery) =>
    //     state => { state.SetArtery(artery); return Cask.Empty(state); };
}
