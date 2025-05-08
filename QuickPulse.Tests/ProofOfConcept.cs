using QuickPulse.Bolts;

namespace QuickPulse.Tests;

public class ProofOfConcept
{
    [Fact]
    public void Run()
    {
        var result = "";
        var flow =
            from x in Pulse.Start<char[]>()
            let y = new string(x)
            from z in Pulse.Trace(() => result = y)
            select Pulse.Stop;
        var signal = Signal.From<char[]>(flow);
        signal.Pulse(['c', 'h']);
        Assert.Equal("ch", result);
    }
}


