using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<T> Prime<T>(Func<T> factory) => s => Cask.Some(s, s.GetTheBox(factory).Value);
}