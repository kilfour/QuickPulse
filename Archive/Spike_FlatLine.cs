using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike_FlatLine
{
    [Fact]
    public void FlatLine_runs_on_terminal_state()
    {

        Assert.True(
            Signal.From(Pulse.Prime(() => false).Dissipate())
            .SetArtery(TheLatch.Holds<bool>())
            .FlatLine(Pulse.Trace(true))
            .GetArtery<Latch<bool>>().Q);
    }
}