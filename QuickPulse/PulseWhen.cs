using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> When(bool flag, Flow<Unit> flow) =>
        state =>
        {
            if (flag)
            {
                flow(state);
                return Cask.Empty(state);
            }
            return Cask.Empty(state);
        };

    public static Flow<Unit> When<T>(Func<T, bool> predicate, Flow<Unit> flow) =>
        state =>
        {
            if (CheckInTheBox(predicate, state))
            {
                flow(state);
                return Cask.Empty(state);
            }
            return Cask.Empty(state);
        };
}