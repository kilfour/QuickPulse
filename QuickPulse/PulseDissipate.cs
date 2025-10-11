namespace QuickPulse;

public static partial class Pulse
{
    public static Flow<Unit> Dissipate<T>(this Flow<T> flow)
        => flow.Then(NoOp());
}