using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike_Valve
{
    [Fact]
    public void Passable()
    {
        var flow =
            from input in Pulse.Start<string>()
            from valve in Pulse.Gather(Valve.Install())
            from _ in Pulse.TraceIf(valve.Value.Passable(), () => "Look ")
            from __ in Pulse.Trace(input)
            select input;
        var holden = TheString.Catcher();
        Signal.From(flow)
            .SetArtery(holden)
            .Pulse("over", " ", "there", ".");
        Assert.Equal("Look over there.", holden.Whispers());
    }

    [Fact]
    public void Restricted()
    {
        var flow =
            from input in Pulse.Start<string>()
            from valve in Pulse.Gather(Valve.Install())
            from _ in Pulse.TraceIf(valve.Value.Restricted(), () => ", ")
            from __ in Pulse.Trace(input)
            select input;
        var holden = TheString.Catcher();
        Signal.From(flow)
            .SetArtery(holden)
            .Pulse("a", "b", "c");
        Assert.Equal("a, b, c", holden.Whispers());
    }

    [Fact]
    public void Open()
    {
        var flow =
            from input in Pulse.Start<string>()
            from valve in Pulse.Gather(Valve.Install())
            from _ in Pulse.TraceIf(valve.Value.Restricted(), () => ", ")
            from __ in Pulse.EffectIf(input == "open", valve.Value.Open)
            from ___ in Pulse.Trace(input)
            select input;
        var holden = TheString.Catcher();
        Signal.From(flow)
            .SetArtery(holden)
            .Pulse("a", "b", "open", "c", "d");
        Assert.Equal("a, b, openc, d", holden.Whispers());
    }

    [Fact]
    public void Using_Scoped()
    {
        var innerFlow1 =
            from input in Pulse.Start<string>()
            from valve in Pulse.Gather<Valve>()
            from _1 in Pulse.TraceIf(valve.Value.Restricted(), () => "-")
            from _2 in Pulse.Trace(input)
            select input;
        var innerFlow2 =
            from input in Pulse.Start<string>()
            from valve in Pulse.Gather<Valve>()
            from _1 in Pulse.TraceIf(valve.Value.Restricted(), () => ", ")
            from _2 in Pulse.Trace(input)
            select input;
        var flow =
            from input in Pulse.Start<string[]>()
            from valve in Pulse.Gather(Valve.Install())
            from _1 in Pulse.Scoped<Valve>(_ => Valve.Install(), Pulse.ToFlow(innerFlow1, input))
            from _2 in Pulse.Trace(" ")
            from _3 in Pulse.Trace(input)
            from _4 in Pulse.Trace(" ")
            from _5 in Pulse.Scoped<Valve>(_ => Valve.Install(), Pulse.ToFlow(innerFlow2, input))
            select input;
        var holden = TheString.Catcher();
        Signal.From(flow)
            .SetArtery(holden)
            .Pulse(["a", "b", "c"]);
        Assert.Equal("a-b-c abc a, b, c", holden.Whispers());
    }
}