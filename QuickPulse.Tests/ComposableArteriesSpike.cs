using QuickPulse.Arteries;
using QuickPulse.Tests.Tools;

namespace QuickPulse.Tests;

public class ComposableArteriesSpike
{
    [Fact]
    public void Initial()
    {
        var collector = Collect.ValuesOf<int>();
        var latch1 = TheLatch.Holds<int>();
        var latch2 = TheLatch.Holds<int>();
        Signal.From(
                from anInt in Pulse.Start<int>()
                from trace in Pulse.Trace(anInt)
                select anInt)
            .SetArtery(collector.Perfuse(latch1, latch2))
            .Pulse([42, 43, 44]);
        Assert.Equal(3, collector.Values.Count);
        Assert.Equal(42, collector.Values[0]);
        Assert.Equal(43, collector.Values[1]);
        Assert.Equal(44, collector.Values[2]);
        Assert.Equal(44, latch1.Q);
        Assert.Equal(44, latch2.Q);
    }
}