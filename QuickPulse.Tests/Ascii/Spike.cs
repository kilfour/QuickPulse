using QuickPulse.Arteries;

namespace QuickPulse.Tests.Ascii;

public class Spike
{
    [Fact]
    public void Go()
    {
        var flow =
            from node in Pulse.Start<Element>()
            select node;

        var signal = Signal.From(flow);
        signal.Pulse(new Node("Root"));

    }
}