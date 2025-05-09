using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace QuickPulse.Tests;

public class CombiningFlowsTests
{
    [Fact]
    public void Combining_ToFlow()
    {
        var innerFlow =
            from anInt in Pulse.Start<int>()
            from t in Pulse.Trace(anInt)
            select anInt;

        var collector = new TheCollector<int>();
        var flow =
            from box in Pulse.Start<Box<int>>()
            from c in Pulse.Using(collector)
            let anInt = box.Value
            from _ in Pulse.ToFlow(innerFlow, anInt)
            select box;

        var signal = Signal.From(flow);
        signal.Pulse(new Box<int>(42));
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }

    [Fact]
    public void Combining_ToFlowIf()
    {
        var innerFlow =
            from anInt in Pulse.Start<int>()
            from t in Pulse.Trace(anInt)
            select anInt;

        var collector = new TheCollector<int>();
        var flow =
            from box in Pulse.Start<Box<int>?>()
            from c in Pulse.Using(collector)
            from _ in Pulse.ToFlowIf(box != null, innerFlow, () => box.Value)
            select box;

        var signal = Signal.From(flow);

        signal.Pulse([null!]);
        Assert.Empty(collector.TheExhibit);

        signal.Pulse(new Box<int>(42));
        Assert.Single(collector.TheExhibit);
        Assert.Equal(42, collector.TheExhibit[0]);
    }
}


