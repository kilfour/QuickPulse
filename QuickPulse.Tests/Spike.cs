using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike
{
    [Fact]
    public void Pipelines()
    {
        var firstFlow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Trace($"{input}")
            select input;

        var secondFlow =
            from input in Pulse.Start<string>()
            from _ in Pulse.Trace($"Second Flow: {input}")
            select input;

        var collector = new TheCollector<string>();
        var signal =
            Signal.From(firstFlow)
                .SetArtery(collector)
                .Then(secondFlow)
                .Pulse(42);

        Assert.Equal("Second Flow: 42", collector.TheExhibit.First());
    }

    [Fact(Skip = "not working")]
    public void Conditionally()
    {
        var firstFlow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Trace(input)
            select input;

        var secondFlow =
            from input in Pulse.Start<int>()
            from _ in Pulse.TraceIf(input == 42, $"{input}")
            select input;

        var thirdFlow =
            from input in Pulse.Start<string>()
            from _ in Pulse.Trace($"Third Flow: {input}")
            select input;

        var collector = new TheCollector<string>();
        var signal =
            Signal.From(firstFlow)
                .SetArtery(collector)
                .Then(secondFlow)
                .Then(thirdFlow)
                .Pulse(666)
                .Pulse(42);

        Assert.Equal("Third Flow: 42", collector.TheExhibit.First());
    }
}