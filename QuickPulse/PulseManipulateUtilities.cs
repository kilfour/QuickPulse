using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State, T> SetTheCell<T>() => (s, v) => s.GetTheCell<T>().Value = v;
    private static Func<State, IEnumerable<T>> ManipulatedValue<T>(Func<T, T> manipulate) =>
        s =>
        {
            var box = s.GetTheCell<T>();
            var next = manipulate(box.Value);
            return [next];
        };
}