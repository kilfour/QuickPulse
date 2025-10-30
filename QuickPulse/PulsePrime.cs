using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Lazily creates and stores a value once per signal lifetime. Use for one-time initialization or memoized state.
    /// </summary>
    public static Flow<TCell> Prime<TCell>(Func<TCell> factory) => s => Beat.Some(s, s.GetTheCell(factory).Value);
}
