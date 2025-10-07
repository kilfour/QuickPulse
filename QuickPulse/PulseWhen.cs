namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> When(bool flag, Flow<Unit> flow) =>
        Runnel(Flag(flag), s => flow(s));
    public static Flow<Unit> When<T>(Func<T, bool> predicate, Flow<Unit> flow) =>
        Runnel(Sluice(predicate), s => flow(s));
    public static Flow<Unit> When(bool flag, Func<Flow<Unit>> flowFactory) =>
        Runnel(Flag(flag), s => flowFactory()(s));
    public static Flow<Unit> When<T>(Func<T, bool> predicate, Func<Flow<Unit>> flowFactory) =>
        Runnel(Sluice(predicate), s => flowFactory()(s));
}