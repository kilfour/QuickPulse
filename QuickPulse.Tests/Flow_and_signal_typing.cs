using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Flow_and_signal_typing
{
    [Fact]
    public void Combining_with_an_int()
    {
        var result = 0;
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Effect(() => result = anInt)
            select anInt;
        var signal = Signal.From(flow);
        signal.Pulse(42);
        Assert.Equal(42, result);
    }

    [Fact]
    public void Combining_with_many_ints()
    {
        var collector = new TheCollector<int>();
        var flow =
            from anInt in Pulse.Start<int>()
            from artery in Pulse.Using(collector)
            from _ in Pulse.Trace(anInt)
            select anInt;

        var signal = Signal.From(flow);
        signal.Pulse([42, 666]);
        Assert.Equal(42, collector.TheExhibit[0]);
        Assert.Equal(666, collector.TheExhibit[1]);
    }
}

