using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{

    public static Flow<Unit> FirstOf(params (Func<bool>, Func<Flow<Unit>>)[] data) =>
        state =>
        {
            foreach (var item in data) if (item.Item1()) return item.Item2()(state);
            return Cask.Empty(state);
        };
}
