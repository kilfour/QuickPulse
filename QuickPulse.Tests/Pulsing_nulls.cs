using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Pulsing_nulls
{
    [Fact]
    public void Baseline()
    {
        var collector = new TheCollector<bool?>();
        var signal = Signal.Tracing<bool?>().SetArtery(collector);
        signal.Pulse(false);
        signal.Pulse(true);

        Assert.Equal(false, collector.TheExhibit[0]);
        Assert.Equal(true, collector.TheExhibit[1]);
    }

    [Fact]
    public void Should_work()
    {
        var collector = new TheCollector<bool?>();
        var signal = Signal.Tracing<bool?>().SetArtery(collector);

        signal.Pulse(null!);
        Assert.Null(collector.TheExhibit[0]);
    }
}

