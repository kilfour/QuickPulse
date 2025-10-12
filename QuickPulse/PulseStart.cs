using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Begins a new flow by pulling the current input value from the signal. Use as the entry point in every LINQ flow.
    /// </summary>
    public static Flow<TOut> Start<TOut>() =>
        s => Cask.Some(s, s.GetValue<TOut>());
}
