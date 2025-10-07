namespace QuickPulse.Tests;

public class Spike_ToFlow_Factory
{
    [Fact]
    public void Example()
    {
        var flow =
            from i in Pulse.Start<int>()
            from c in Pulse.Gather(42)
            from t in Pulse.ToFlow(a => Pulse.Manipulate<int>(a => a + 1).AsUnit(), i)
            select i;
        // var text =
        //     Signal.From(flow)
        //         .SetArtery(TheString.Catcher())
        //         .Pulse(Lines())
        //         .GetArtery<Holden>()
        //         .Whispers();
        // Assert.Equal("onetwo", text);
    }
}

public static class FlowExt
{
    public static Flow<Unit> AsUnit<T>(this Flow<T> flow)
        => from _ in flow select Unit.Instance;
}