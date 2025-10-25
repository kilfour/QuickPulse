using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static Action<State, object> IntoGraftedArtery<TArtery>() where TArtery : IArtery =>
        (s, o) => s.GetArtery<TArtery>().Absorb(o);
}
