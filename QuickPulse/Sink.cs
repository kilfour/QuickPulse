using QuickPulse.Bolts;

namespace QuickPulse;

public static class Sink
{
    public static Flow<Unit> To(Action action) =>
        _ => { action(); return Cask.Empty(); };
}
