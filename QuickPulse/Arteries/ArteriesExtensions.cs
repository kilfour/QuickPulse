using QuickPulse.Arteries.Bolts;

namespace QuickPulse.Arteries;

public static class ArteriesExtensions
{
    /// <summary>
    /// Routes absorbed pulses sequentially through the given arteries,
    /// passing the output of each artery into the next.
    /// Use to build a perfusion path where each artery may observe or transform the flow
    /// before it reaches the next stage.
    /// </summary>
    public static IArtery Perfuse(this IArtery artery, params IArtery[] branches) =>
        new ConduitArtery(artery, branches);
}
