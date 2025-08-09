using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Box<T>> Gather<T>(T value) => s => Cask.Some(s, s.GetTheBox(value));
    public static Flow<Box<T>> Gather<T>() => s => Cask.Some(s, s.GetTheBox<T>());
}