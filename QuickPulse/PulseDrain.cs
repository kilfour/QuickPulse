namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> Drain<T>(Func<T, Flow<Unit>> step)
        => Drain<T, Unit>(step);

    public static Flow<Unit> Drain<T, TInner>(Func<T, Flow<TInner>> step)
        => Circulate(step).Dissipate();
}