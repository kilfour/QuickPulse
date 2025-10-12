using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State, T> SetTheBox<T>() => (s, v) => s.GetTheBox<T>().Value = v;
    private static Func<State, IEnumerable<T>> ManipulatedValue<T>(Func<T, T> manipulate) =>
        s =>
        {
            var box = s.GetTheBox<T>();
            var next = manipulate(box.Value);
            return [next];
        };
}