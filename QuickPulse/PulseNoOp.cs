using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> NoOp() => Cask.Empty;
}