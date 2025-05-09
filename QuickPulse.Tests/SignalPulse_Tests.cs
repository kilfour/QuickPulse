

namespace QuickPulse.Tests;

public class SignalPulse_Tests
{
    [Fact]
    public void Pulsing_without_a_value()
    {
        var result = 0;
        var flow =
            from anInt in Pulse.Start<int>()
            from _ in Pulse.Effect(() => result = anInt)
            select anInt;
        var signal = Signal.From(flow);
        signal.Pulse();
    }

    [Fact]
    public void Pulsing_without_a_value_but_used()
    {
        var result = 0;
        var flow =
            from anInt in Pulse.Start<int>()
            let add = anInt + 1
            from _ in Pulse.Effect(() => result = add)
            select anInt;
        var signal = Signal.From(flow);
        signal.Pulse();
    }
}

