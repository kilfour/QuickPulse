using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> StopFlowing() => HandMeACask(s => s.StopFlowing());

    public static Flow<Unit> StopFlowingIf(bool condition) =>
    condition ? StopFlowing() : NoOp();

    public static Flow<Unit> StopFlowingIf<T>(Func<T, bool> predicate) =>
        When(predicate, StopFlowing());
}