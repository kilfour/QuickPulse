using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<T> Manipulate<T>(Func<T, T> manipulate) =>
        state =>
        {
            var box = GetTheBox<T>(state);
            box.Value = manipulate(box.Value);
            return Cask.Some(state, box.Value);
        };

    public static Flow<T> ManipulateIf<T>(bool flag, Func<T, T> manipulate) =>
        state => flag ? Manipulate(manipulate)(state) : Cask.None<T>(state);
}