using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State> WithScope<TValue>(Func<TValue, TValue> enter, Flow<Unit> flow) =>
        state =>
        {
            var box = state.GetTheBox<TValue>();
            var prev = box.Value;
            box.Value = enter(prev);
            try { flow(state); }
            finally { box.Value = prev; }
        };

    public static Flow<Unit> Scoped<TValue>(Func<TValue, TValue> enter, Flow<Unit> flow) =>
        Runnel(Always, WithScope(enter, flow));

    public static Flow<Unit> Scoped<TValue>(TValue value, Flow<Unit> flow) =>
        Scoped<TValue>(_ => value, flow);

    public static Flow<Unit> Scoped<TValue>(Func<TValue> compute, Flow<Unit> flow) =>
        Scoped<TValue>(_ => compute(), flow);

    public static Flow<Unit> ScopedIf<TValue>(bool flag, Func<TValue, TValue> enter, Flow<Unit> flow) =>
        Runnel(Flag(flag), WithScope(enter, flow));

    public static Flow<Unit> ScopedIf<TValue, TBox>(Func<TBox, bool> predicate, Func<TValue, TValue> enter, Flow<Unit> flow) =>
        Runnel(Sluice(predicate), WithScope(enter, flow));
}