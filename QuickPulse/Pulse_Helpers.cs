using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    // --------------------------------------------------------------------------------
    // -- Gates
    // -- 
    private static Func<State, bool> Always => _ => true;
    private static Func<State, bool> Flag(bool flag) => _ => flag;
    private static Func<State, bool> Sluice<TBox>(Func<TBox, bool> predicate) =>
        s => predicate(s.GetTheBox<TBox>().Value);
    // --------------------------------------------------------------------------------
    // -- Value Shapers
    // --
    private static Func<State, IEnumerable<T>> Single<T>(T value) => _ => new[] { value };
    private static Func<State, IEnumerable<T>> Single<T>(Func<T> value) => _ => new[] { value() };
    private static Func<State, IEnumerable<T>> Many<T>(IEnumerable<T> values) => _ => values;
    private static Func<State, IEnumerable<T>> Many<T>(Func<IEnumerable<T>> values) => _ => values();
    // --------------------------------------------------------------------------------
    // -- Runnels
    // --
    private static Flow<Unit> HandMeACask(Action<State> action) => s => { action(s); return Cask.Empty(s); };
    private static Flow<Unit> Runnel(Func<State, bool> shouldRun, Action<State> effect) =>
        HandMeACask(s => { if (shouldRun(s)) effect(s); });
    private static Flow<Unit> Runnel<TValue>(Func<State, bool> shouldRun, Func<State, TValue> value, Action<State, TValue> action) =>
        HandMeACask(s => { if (shouldRun(s)) action(s, value(s)); });
    private static Flow<Unit> Runnel<TValue>(Func<State, bool> shouldRun, Func<State, IEnumerable<TValue>> values, Action<State, TValue> action) =>
        HandMeACask(s => { if (shouldRun(s)) foreach (var v in values(s)) { action(s, v); } });
    private static Flow<TValue> Fyke<TValue>(Func<State, bool> shouldRun, Func<State, IEnumerable<TValue>> values, Action<State, TValue> action) =>
        s =>
        {
            if (!shouldRun(s)) return Cask.None<TValue>(s);
            bool any = false;
            TValue? last = default;
            foreach (var v in values(s))
            {
                any = true;
                last = v;
                action(s, v);
            }
            return any ? Cask.Some(s, last!) : Cask.None<TValue>(s);
        };
    // --------------------------------------------------------------------------------
    // -- Flow Factory
    // --
    internal static Flow<T> GetFlowFromFactory<T>(Func<T, Flow<Unit>> flowFactory) =>
        from i in Start<T>() from _ in flowFactory(i) select i;
}