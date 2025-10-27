using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Runs the first subflow whose predicate returns true. Use to express prioritized branching without nesting.
    /// </summary>
    public static Flow<Flow> FirstOf(params (Func<bool> Predicate, Func<Flow<Flow>> FlowFactory)[] data) =>
        s =>
        {
            foreach (var item in data) if (item.Predicate()) return item.FlowFactory()(s);
            return Beat.Empty(s);
        };

    /// <summary>
    /// Runs the first subflow whose predicate returns true for the current boxed state value. Use for state-driven conditional routing.
    /// </summary>
    public static Flow<Flow> FirstOf<T, TCell>(params (Func<TCell, bool> Predicate, Func<Flow<Flow>> FlowFactory)[] data) =>
        s =>
        {
            var box = s.GetTheCell<TCell>().Value;
            foreach (var item in data) if (item.Predicate(box)) return item.FlowFactory()(s);
            return Beat.Empty(s);
        };
}
