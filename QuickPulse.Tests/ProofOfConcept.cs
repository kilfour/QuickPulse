using QuickPulse.Bolts;
using QuickPulse.Diagnostics;

namespace QuickPulse.Tests;

public class ProofOfConcept
{
    [Fact]
    public void InjectedValue()
    {
        var result = "";
        var pulse =
            from x in Pulse.Start<char[]>()
            let y = new string(x)
            from z in Pulse.Trace(() => result = y)
            select Pulse.Stop;
        char[] input = ['c', 'h'];
        pulse.Run(input);
        Assert.Equal("ch", result);
    }

    [Fact]
    public void BoundPulse()
    {
        var result = "";
        var pulse =
            from x in Pulse.Start<char[]>(['c', 'h'])
            let y = new string(x)
            from z in Pulse.Trace(() => result = y)
            select Unit.Instance;
        pulse.RunBoundPulse();
        Assert.Equal("ch", result);
    }

    [Fact]
    public void WithContext()
    {
        var result = "";
        PulseContext.Current =
            (from x in Pulse.Start<char[]>(['c', 'h'])
             let y = new string(x)
             from z in Pulse.Trace(() => result = y)
             select x).ToPulse();
        char[] input = ['c', 'h'];
        PulseContext.Log(input);
        Assert.Equal("ch", result);
    }

    [Fact]
    public void WherePassed()
    {
        var result = "";
        PulseContext.Current =
            (from x in Pulse.Start<char[]>(['c', 'h', 'a', 'r'])
             let y = new string(x)
             from z in Pulse.Trace(() => result = y)
             where x.Contains('a')
             select x).ToPulse();
        char[] input = ['c', 'h'];
        PulseContext.Log(input);
        Assert.Equal("char", result);
    }

    [Fact]
    public void Where()
    {
        var result = "";
        PulseContext.Current =
            (from x in Pulse.Start<char[]>(['c', 'h', 'a', 'r'])
             let y = new string(x)
             from z in Pulse.Trace(() => result = y)
             where x.Contains('q')
             select Pulse.Stop).ToPulse();
        char[] input = ['c', 'h'];
        PulseContext.Log(input);
        Assert.Equal("char", result);
    }

    [Fact]
    public void WhereFailedAgain()
    {
        var result = "";
        PulseContext.Current =
            (from x in Pulse.Start<char[]>(['c', 'h', 'a', 'r'])
             where x.Contains('q')
             let y = new string(x)
             from z in Pulse.Trace(() => result = y)
             select x).ToPulse();
        char[] input = ['c', 'h'];
        PulseContext.Log(input);
        Assert.Equal("", result);
    }
}


