using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State, TCell> SetTheCell<TCell>() => (s, v) => s.GetTheCell<TCell>().Value = v;
    private static Func<State, IEnumerable<TCell>> ManipulatedValue<TCell>(Func<TCell, TCell> manipulate) =>
        s =>
        {
            var box = s.GetTheCell<TCell>();
            var next = manipulate(box.Value);
            return [next];
        };
}