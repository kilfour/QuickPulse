using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static readonly Action<State, object> IntoArtery =
        (s, o) => s.CurrentArtery!.Absorb(o);
}
