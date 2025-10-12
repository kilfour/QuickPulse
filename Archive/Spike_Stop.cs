using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike_Stop
{
    private IEnumerable<string> Lines()
    {
        yield return "one";
        yield return "two";
        throw new Exception("BOOM");
    }

    [Fact]
    public void BasedOnState()
    {
        var flow =
            from i in Pulse.Start<string>()
            from t in Pulse.Trace(i)
            from s in Pulse.StopFlowingIf(i == "two")
            select i;
        var text =
            Signal.From(flow)
                .SetArtery(TheString.Catcher())
                .Pulse(Lines())
                .GetArtery<Holden>()
                .Whispers();
        Assert.Equal("onetwo", text);
    }
}
