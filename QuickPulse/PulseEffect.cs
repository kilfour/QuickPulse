using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> Effect(Action action) =>
        state => { action(); return Cask.Empty(state); };

    public static Flow<Unit> EffectIf(bool flag, Action action) =>
        state =>
        {
            if (flag) action();
            return Cask.Empty(state);
        };

    public static Flow<Unit> EffectIf<T>(Func<T, bool> predicate, Action action) =>
        state =>
        {
            if (CheckInTheBox(predicate, state))
                action();
            return Cask.Empty(state);
        };
}