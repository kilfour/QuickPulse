using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class TransformTests
{
    [Fact]
    public void Manipulating_flow_actions()
    {
        var collector = new TheCollector<string>();
        var flow =
            from str in Pulse.Start<string>()
            from act in Pulse.Gather<Action>(() => { })
            from _ in Pulse.Trace(str)
            select str;
        var signal = Signal.From(flow);
        signal.Pulse("One");
        signal.Pulse("Two");
    }
}


