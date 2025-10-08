using QuickPulse.Arteries;

namespace QuickPulse.Tests;

public class Spike_OnType
{
    public abstract class Base { }
    public class DerivedOne : Base { }
    public class DerivedTwo : Base { }
    public class DerivedThree : Base { }

    [Fact]
    public void Riffing()
    {
        var one =
            from start in Pulse.Start<DerivedOne>()
            from _ in Pulse.Trace("DerivedOne")
            select start;
        var two =
            from start in Pulse.Start<DerivedTwo>()
            from _ in Pulse.Trace("DerivedTwo")
            select start;
        var flow =
            from input in Pulse.Start<Base>()
            from _ in Pulse.Into(one, input)
            from __ in Pulse.Into(two, input)
            from ___ in Pulse.Trace(" ")
            select input;
        var text =
            Signal.From(flow)
                .SetArtery(TheString.Catcher())
                .Pulse([new DerivedOne(), new DerivedTwo(), new DerivedThree()])
                .GetArtery<Holden>()
                .Whispers();
        Assert.Equal("DerivedOne DerivedTwo  ", text);
    }
}
