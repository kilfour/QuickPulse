using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike_Valve
{
    [Fact]
    public void Passable()
    {
        var flow =
            from input in Pulse.Start<string>()
            from valve in Pulse.Prime(Valve.Install)
            from _ in Pulse.TraceIf(valve.Passable(), () => "Look ")
            from __ in Pulse.Trace(input)
            select input;
        var stringSink = Text.Capture();
        Signal.From(flow)
            .SetArtery(stringSink)
            .Pulse(["over", " ", "there", "."]);
        Assert.Equal("Look over there.", stringSink.Content());
    }

    [Fact]
    public void Restricted()
    {
        var flow =
            from input in Pulse.Start<string>()
            from valve in Pulse.Prime(Valve.Install)
            from _ in Pulse.TraceIf(valve.Restricted(), () => ", ")
            from __ in Pulse.Trace(input)
            select input;
        var stringSink = Text.Capture();
        Signal.From(flow)
            .SetArtery(stringSink)
            .Pulse(["a", "b", "c"]);
        Assert.Equal("a, b, c", stringSink.Content());
    }

    [Fact]
    public void Open()
    {
        var flow =
            from input in Pulse.Start<string>()
            from valve in Pulse.Prime(Valve.Install)
            from _ in Pulse.TraceIf(valve.Restricted(), () => ", ")
            from __ in Pulse.ManipulateIf<Valve>(input == "open", a => a.Open())
            from ___ in Pulse.Trace(input)
            select input;
        var stringSink = Text.Capture();
        Signal.From(flow)
            .SetArtery(stringSink)
            .Pulse(["a", "b", "open", "c", "d"]);
        Assert.Equal("a, b, openc, d", stringSink.Content());
    }

    [Fact]
    public void Using_Scoped()
    {
        var innerFlow1 =
            from input in Pulse.Start<string>()
            from valve in Pulse.Draw<Valve>()
            from _1 in Pulse.TraceIf(valve.Restricted(), () => "-")
            from _2 in Pulse.Trace(input)
            select input;
        var innerFlow2 =
            from input in Pulse.Start<string>()
            from valve in Pulse.Draw<Valve>()
            from _1 in Pulse.TraceIf(valve.Restricted(), () => ", ")
            from _2 in Pulse.Trace(input)
            select input;
        var flow =
            from input in Pulse.Start<string[]>()
            from valve in Pulse.Prime(Valve.Install)
            from _1 in Pulse.Scoped<Valve>(_ => Valve.Install(), Pulse.ToFlow(innerFlow1, input))
            from _2 in Pulse.Trace(" ")
            from _3 in Pulse.Trace(input)
            from _4 in Pulse.Trace(" ")
            from _5 in Pulse.Scoped<Valve>(_ => Valve.Install(), Pulse.ToFlow(innerFlow2, input))
                // from _5 in Pulse.ToFlow(innerFlow2, input) == same result
            select input;
        var stringSink = Text.Capture();
        Signal.From(flow)
            .SetArtery(stringSink)
            .Pulse(["a", "b", "c"]);
        Assert.Equal("a-b-c abc a, b, c", stringSink.Content());
    }
}