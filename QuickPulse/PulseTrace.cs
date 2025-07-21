using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> Trace(params object[] data) =>
        state =>
        {
            state.CurrentArtery?.Flow(data);
            return Cask.Empty(state);
        };

    public static Flow<Unit> TraceIf(bool flag, Func<object> data) =>
        state =>
        {
            if (flag)
                state.CurrentArtery?.Flow(data());
            return Cask.Empty(state);
        };

    public static Flow<Unit> TraceIf<T>(Func<T, bool> predicate, Func<object> data) =>
        state =>
        {
            if (CheckInTheBox(predicate, state))
                state.CurrentArtery?.Flow(data());
            return Cask.Empty(state);
        };
}
