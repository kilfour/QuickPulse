namespace QuickPulse.Arteries.Bolts;

/// <summary>
/// An <see cref="IArtery"/> that perfuses pulses through a sequence of arteries,
/// feeding the output of each artery into the next.
/// Use to model a linear arterial pathway where each stage may observe or
/// transform the flowing data before it continues downstream.
/// </summary>
public class ConduitArtery(IArtery artery, IArtery[] branches) : IArtery
{
    public object[] Absorb(params object[] data) =>
        branches.Aggregate(artery.Absorb(data), (acc, a) => a.Absorb(acc));
}
