using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// A flow that does nothing and emits nothing. Use as a neutral placeholder or empty branch.
    /// </summary>
    public static Flow<Unit> NoOp() => Cask.Empty;
}
