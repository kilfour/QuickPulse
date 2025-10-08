using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class FlowEnumerationBugTests
{
    [Fact]
    public void Reproduction()
    {
        var inner =
            from i in Pulse.Start<int>()
            from _ in Pulse.Trace(i)
            select i;

        var flow =
            from i in Pulse.Start<IEnumerable<int>>()
            from t in Pulse.ToFlow(inner, i)
            select i;

        var result =
            Signal.From(flow)
                .SetArtery(new TheCollector<int>())
                .Pulse([1, 2])
                .GetArtery<TheCollector<int>>()
                .TheExhibit;
        Assert.Equal(1, result[0]);
        Assert.Equal(2, result[1]);
    }

    [Fact]
    public void Scoped_executes_inner_and_threads_returned_state()
    {
        var flow =
            from _ in Pulse.Gather(0)
            from __ in Pulse.Scoped<int>(m => m, Pulse.Trace("inside"))
            select Unit.Instance;

        var signal = Signal.From(flow)
            .SetArtery(new TheCollector<string>())
            .Pulse(Unit.Instance);

        Assert.Equal("inside",
            signal.GetArtery<TheCollector<string>>().TheExhibit.Single());
    }
}