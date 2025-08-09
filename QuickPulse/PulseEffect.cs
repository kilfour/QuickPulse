using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> Effect(Action action) =>
        Runnel(Always, _ => action());

    public static Flow<Unit> EffectIf(bool flag, Action action) =>
        Runnel(Flag(flag), _ => action());

    public static Flow<Unit> EffectIf<T>(Func<T, bool> predicate, Action action) =>
        Runnel(Sluice(predicate), _ => action());
}