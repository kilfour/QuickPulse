using QuickPulse.Diagnostics;

namespace QuickPulse.Tests;

public class ProofOfConcept
{
    [Fact]
    public void InjectedValue()
    {
        var result = "";
        var pulse =
            from x in Pulse.From<char[]>()
            from y in Pulse.Shape(() => new string(x))
            from z in Sink.To(() => result = y)
            select x;
        char[] input = ['c', 'h'];
        pulse.Run(input);
        Assert.Equal("ch", result);
    }

    [Fact]
    public void BoundPulse()
    {
        var result = "";
        var pulse =
            from x in Pulse.From<char[]>(['c', 'h'])
            from y in Pulse.Shape(() => new string(x))
            from z in Sink.To(() => result = y)
            select x;
        pulse.RunBoundPulse();
        Assert.Equal("ch", result);
    }

    [Fact]
    public void WithContext()
    {
        var result = "";
        PulseContext.Current =
            (from x in Pulse.From<char[]>(['c', 'h'])
             from y in Pulse.Shape(() => new string(x))
             from z in Sink.To(() => result = y)
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
            (from x in Pulse.From<char[]>(['c', 'h', 'a', 'r'])
             from y in Pulse.Shape(() => new string(x))
             from z in Sink.To(() => result = y)
             where x.Contains('a')
             select x).ToPulse();
        char[] input = ['c', 'h'];
        PulseContext.Log(input);
        Assert.Equal("char", result);
    }

    [Fact]
    public void WhereFailed()
    {
        var result = "";
        PulseContext.Current =
            (from x in Pulse.From<char[]>(['c', 'h', 'a', 'r'])
             from y in Pulse.Shape(() => new string(x))
             from z in Sink.To(() => result = y)
             where x.Contains('q')
             select x).ToPulse();
        char[] input = ['c', 'h'];
        PulseContext.Log(input);
        Assert.Equal("char", result);
    }

    [Fact]
    public void WhereFailedAgain()
    {
        var result = "";
        PulseContext.Current =
            (from x in Pulse.From<char[]>(['c', 'h', 'a', 'r'])
             where x.Contains('q')
             from y in Pulse.Shape(() => new string(x))
             from z in Sink.To(() => result = y)
             select x).ToPulse();
        char[] input = ['c', 'h'];
        PulseContext.Log(input);
        Assert.Equal("", result);
    }
}


