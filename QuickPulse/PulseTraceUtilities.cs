using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    private static readonly Action<State, object> IntoArtery =
        (s, o) => s.CurrentArtery!.Absorb(o);
    private static Func<State, object> ExtractDataFromCell<TCell>(Func<TCell, object> data) =>
        s => { var v = s.GetTheCell<TCell>().Value; return data(v); };
}
