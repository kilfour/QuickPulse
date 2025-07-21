using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{

    public static Flow<Box<T>> Gather<T>(T value) =>
        state =>
        {
            if (!state.Memory.TryGetValue(typeof(T), out var obj))
            {
                var box = new Box<T>(value);
                state.Memory[typeof(T)] = box;
                return Cask.Some(state, box);
            }
            return Cask.Some(state, (Box<T>)obj!);
        };

    public static Flow<Box<T>> Gather<T>() =>
        state => Cask.Some(state, GetTheBox<T>(state));
}