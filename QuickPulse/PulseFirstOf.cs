using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> FirstOf(params (Func<bool> Predicate, Func<Flow<Unit>> FlowFactory)[] data) =>
        s =>
        {
            foreach (var item in data) if (item.Predicate()) return item.FlowFactory()(s);
            return Cask.Empty(s);
        };

    public static Flow<Unit> FirstOf<T, TBox>(params (Func<TBox, bool> Predicate, Func<Flow<Unit>> FlowFactory)[] data) =>
        s =>
        {
            var box = s.GetTheBox<TBox>().Value;
            foreach (var item in data) if (item.Predicate(box)) return item.FlowFactory()(s);
            return Cask.Empty(s);
        };
}
