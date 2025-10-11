using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike_Manipulate
{
    [Fact]
    public void Simple()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Prime(() => 0)
            from cnt in Pulse.Manipulate<int>(a => a + 1)
            from ___ in Pulse.Trace($"{cnt + input} ")
            select input;
        var holden = TheString.Catcher();
        Signal.From(flow)
            .SetArtery(holden)
            .Pulse([40, 40, 40, 40]);
        Assert.Equal("41 42 43 44 ", holden.Whispers());
    }
}