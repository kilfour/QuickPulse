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
    public static Flow<T> Manipulate<T>(Func<T, T> manipulate) =>
        Fyke(Always, ManipulatedValue(manipulate), SetTheBox<T>());
    public static Flow<T> ManipulateIf<T>(bool flag, Func<T, T> manipulate) =>
        Fyke(Flag(flag), ManipulatedValue(manipulate), SetTheBox<T>());
    public static Flow<T> ManipulateIf<T>(Func<T, bool> predicate, Func<T, T> manipulate) =>
        Fyke(Sluice(predicate), ManipulatedValue(manipulate), SetTheBox<T>());
}