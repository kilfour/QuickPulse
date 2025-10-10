using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<T> Extract<T>() => s => Cask.Some(s, s.GetTheBox<T>().Value);
}