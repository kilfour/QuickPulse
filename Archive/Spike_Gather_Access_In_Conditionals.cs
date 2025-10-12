using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike_Gather_Access_In_Conditionals
{
    [Fact]
    public void When()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Prime(() => Valve.Install())
            from conditional in Pulse.When<Valve>(a => a.Restricted(), Pulse.Trace($"{input} "))
            select input;
        var holden = TheString.Catcher();
        Signal.From(flow)
            .SetArtery(holden)
            .Pulse([1, 2, 3, 4]);
        Assert.Equal("2 3 4 ", holden.Whispers());
    }

    [Fact]
    public void TraceIf()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Prime(() => Valve.Install())
            from conditional in Pulse.TraceIf<Valve>(a => a.Restricted(), () => $"{input} ")
            select input;
        var holden = TheString.Catcher();
        Signal.From(flow)
            .SetArtery(holden)
            .Pulse([1, 2, 3, 4]);
        Assert.Equal("2 3 4 ", holden.Whispers());
    }

    [Fact]
    public void ToFlowIf()
    {
        var subFlow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Trace($"{input} ")
            select input;

        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Prime(() => Valve.Install())
            from conditional in Pulse.ToFlowIf<int, Valve>(a => a.Restricted(), subFlow, () => input)
            select input;
        var holden = TheString.Catcher();
        Signal.From(flow)
            .SetArtery(holden)
            .Pulse([1, 2, 3, 4]);
        Assert.Equal("2 3 4 ", holden.Whispers());
    }

    [Fact]
    public void EffectIf()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(Valve.Install)
            from _2 in Pulse.Prime(() => 0)
            from _3 in Pulse.When<Valve>(a => a.Passable(), Pulse.Manipulate<int>(a => a + 1).Dissipate())
            from _4 in Pulse.Trace<int>(a => $"{a}:{input} ")
            select input;
        var holden = TheString.Catcher();
        Signal.From(flow)
            .SetArtery(holden)
            .Pulse([1, 2, 3, 4]);
        Assert.Equal("1:1 1:2 1:3 1:4 ", holden.Whispers());
    }
}