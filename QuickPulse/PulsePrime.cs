using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Lazily creates and stores a value once per signal lifetime. Use for one-time initialization or memoized state.
    /// </summary>
    public static Flow<T> Prime<T>(Func<T> factory) => s => Beat.Some(s, s.GetTheCell(factory).Value);
}
