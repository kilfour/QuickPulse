using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike_SweeterStill
{
    [Fact]
    public void Sugaring()
    {
        var collector = new TheCollector<string>();
        Signal.From<int>(a => Pulse.Trace($"T: {a}"))
            .SetArtery(collector)
            .Pulse(42);
        Assert.Equal(["T: 42"], collector.TheExhibit);

        // same done for ToFlow and ToFlowIf 
    }
}