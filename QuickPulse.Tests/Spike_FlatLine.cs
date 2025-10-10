using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike_FlatLine
{
    [Fact]
    public void FlatLine_runs_on_terminal_state()
    {
        var seen = false;

        var flow =
            from x in Pulse.Start<int>()
            from box in Pulse.Gather(42)
            select x;

        Signal.From(flow)
            .SetArtery(Install.Shunt)
            .Pulse(0)
            .FlatLine(
                from s in Pulse.Gather<int>()
                from _ in Pulse.Effect(() => seen = s.Value == 42)
                select Unit.Instance
            );

        Assert.True(seen);
    }
}