using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<T> Draw<T>() => s => Cask.Some(s, s.GetTheBox<T>().Value);

    //public static Flow<Y> Extract<T, Y>(Func<T, Y> extractor) => s => Cask.Some(s, extractor(s.GetTheBox<T>().Value));
}