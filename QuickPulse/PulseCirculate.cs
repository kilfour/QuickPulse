namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<T> Circulate<T>(Func<T, Flow<Unit>> step) =>
        Circulate<T, Unit>(step);

    public static Flow<T> Circulate<T, TInner>(Func<T, Flow<TInner>> step) =>
        from x in Start<T>()
        from _ in step(x)
        select x;
}