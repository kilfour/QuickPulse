using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    // --------------------------------------------------------------------------------
    // -- Gates
    // -- 
    private static Func<State, bool> Always => _ => true;
    private static Func<State, bool> Flag(bool flag) => _ => flag;
    private static Func<State, bool> Gate<TCell>(Func<TCell, bool> predicate) =>
        s => predicate(s.GetTheCell<TCell>().Value);
    // --------------------------------------------------------------------------------
    // -- Value Shapers
    // --
    private static Func<State, IEnumerable<T>> Single<T>(T value) => _ => [value];
    private static Func<State, IEnumerable<T>> Single<T>(Func<T> value) => _ => [value()];
    private static Func<State, IEnumerable<T>> Many<T>(IEnumerable<T> values) => _ => values;
    private static Func<State, IEnumerable<T>> Many<T>(Func<IEnumerable<T>> values) => _ => values();
    // --------------------------------------------------------------------------------
    // -- Runnels
    // --
    private static Flow<Flow> GiveMeABeat(Action<State> action) => s => { action(s); return Beat.Empty(s); };
    private static Flow<Flow> Emit(Func<State, bool> shouldRun, Action<State> effect) =>
        GiveMeABeat(s => { if (shouldRun(s)) effect(s); });
    private static Flow<Flow> Emit<TValue>(Func<State, bool> shouldRun, Func<State, TValue> value, Action<State, TValue> action) =>
        GiveMeABeat(s => { if (shouldRun(s)) action(s, value(s)); });
    private static Flow<Flow> Emit<TValue>(Func<State, bool> shouldRun, Func<State, IEnumerable<TValue>> values, Action<State, TValue> action) =>
        GiveMeABeat(s => { if (shouldRun(s)) foreach (var v in values(s)) { action(s, v); } });
    private static Flow<TValue> Transduce<TValue>(Func<State, bool> shouldRun, Func<State, IEnumerable<TValue>> values, Action<State, TValue> action) =>
        s =>
        {
            if (!shouldRun(s)) return Beat.None<TValue>(s);
            bool any = false;
            TValue? last = default;
            foreach (var v in values(s))
            {
                any = true;
                last = v;
                action(s, v);
            }
            return any ? Beat.Some(s, last!) : Beat.None<TValue>(s);
        };
    // --------------------------------------------------------------------------------
    // -- Flow Factory
    // --
    internal static Flow<T> GetFlowFromFactory<T>(Func<T, Flow<Flow>> flowFactory) =>
        from i in Start<T>() from _ in flowFactory(i) select i;
}