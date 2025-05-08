using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class TransformTests
{
    [Fact]
    public void CollectStrings()
    {
        List<string> collector = [];
        var flow =
            from str in Pulse.Start<string>()
            from _ in Pulse.Effect(() => collector.Add(str))
            select Pulse.Stop;
        var signal = Signal.From<string>(flow);
        signal.Pulse("One");
        signal.Pulse("Two");
        Assert.Equal(2, collector.Count);
        Assert.Equal("One", collector[0]);
        Assert.Equal("Two", collector[1]);
    }

    [Fact]
    public void CollectStringsAgain()
    {
        var collector = new TheCollector<string>();
        var flow =
            from str in Pulse.Start<string>()
            from c in Pulse.Using(collector)
            from _ in Pulse.Trace(str)
            select Pulse.Stop;
        var signal = Signal.From<string>(flow);
        signal.Pulse("One");
        signal.Pulse("Two");
        Assert.Equal(2, collector.Exhibit.Count);
        Assert.Equal("One", collector.Exhibit[0]);
        Assert.Equal("Two", collector.Exhibit[1]);
    }

    [Fact]
    public void CollectStringsAgainAgain()
    {
        var collector = new TheCollector<string>();
        var flow =
            from str in Pulse.Start<string>()
            from a in Pulse.Using(collector)
            from _ in Pulse.Trace(str)
            select Pulse.Stop;
        var signal = Signal.From<string>(flow);
        signal.Pulse("One");
        signal.Pulse("Two");
        Assert.Equal(2, collector.Exhibit.Count);
        Assert.Equal("One", collector.Exhibit[0]);
        Assert.Equal("Two", collector.Exhibit[1]);
    }

    [Fact]
    public void PullingInState()
    {
        List<string> collector = [];
        var flow =
            from str in Pulse.Start<string>()
            from box in Pulse.Gather(0)
            from _ in Pulse.EffectIf(
                box.Value == 0,
                () => collector.Add(str))
            select Pulse.Stop;
        var signal = Signal.From<string>(flow);
        signal.Pulse("One");
        signal.Manipulate<int>(a => a + 1);
        signal.Pulse("Two");
        signal.Manipulate<int>(a => a - 1);
        signal.Pulse("Three");
        Assert.Equal(2, collector.Count);
        Assert.Equal("One", collector[0]);
        Assert.Equal("Three", collector[1]);
    }
}


