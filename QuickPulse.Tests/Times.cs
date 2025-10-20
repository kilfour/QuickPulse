using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Tests;


public static class TimesExtensions
{
    public static void Times(this int times, Action action) =>
        Signal.From<Unit>(a => Pulse.Trace(Chain.It(action, Unit.Instance)))
            .Pulse(Enumerable.Repeat(Unit.Instance, times));

    public static IEnumerable<T> Times<T>(this int times, Func<T> func) =>
        Signal.From<Unit>(a => Pulse.Trace(func()!))
            .GetResult<T>(times);

    public static IEnumerable<T> TimesUntil<T>(this int times, Predicate<T> predicate, Func<T> func) =>
        Signal.From<Unit>(a =>
                from i in Pulse.Start<Unit>()
                let value = func()
                let stop = predicate(value)
                from _1 in Pulse.TraceIf(!stop, () => value)
                from _2 in Pulse.StopFlowingIf(stop)
                select i)
            .GetResult<T>(times);

    private static List<T> GetResult<T>(this Signal<Unit> signal, int times) =>
        signal
            .SetArtery(TheCollector.Exhibits<T>())
            .Pulse(Enumerable.Repeat(Unit.Instance, times))
            .GetArtery<Collector<T>>()
            .TheExhibit;
}

public class TimesTests
{
    [Fact]
    public void Times_ActionRuns_ExactNumberOfTimes_WhenPositive()
    {
        int calls = 0;
        5.Times(() => { calls++; });
        Assert.Equal(5, calls);
    }

    [Fact]
    public void Times_ActionDoesNotRun_WhenZero()
    {
        int calls = 0;
        0.Times(() => { calls++; });
        Assert.Equal(0, calls);
    }

    [Fact]
    public void Times_Throws_ArgumentOutOfRangeException_When_Negative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => (-3).Times(() => { }));
    }

    [Fact]
    public void Times_NullAction_ThrowsNullReferenceException()
        => Assert.Throws<NullReferenceException>(() => 1.Times(null!));

    [Fact]
    public void TimesT_Yields_ExactNumberOfElements()
    {
        var seq = 4.Times(() => 42).ToList();
        Assert.Equal(4, seq.Count);
        Assert.True(seq.All(x => x == 42));
    }

    [Fact]
    public void TimesT_YieldsEmpty_WhenZero()
    {
        Assert.Empty(0.Times(() => 1));
    }

    [Fact]
    public void TimesT_Throws_ArgumentOutOfRangeException_When_Negative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => (-3).Times(() => 1));
    }

    [Fact]
    public void TimesT_IsEager()
    {
        int calls = 0;
        IEnumerable<int> seq = 3.Times(() => { calls++; return 1; });

        Assert.Equal(3, calls);
        var list = seq.ToList();
        Assert.Equal(3, calls);
        Assert.Equal([1, 1, 1], list);
    }

    [Fact]
    public void TimesT_NullFunc_Throws()
    {
        Assert.Throws<NullReferenceException>(() => 2.Times(null!));
    }

    [Fact]
    public void TimesUntil_StopsEarly_WhenPredicateBecomesTrue()
    {
        int n = 0;
        var produced = 10.TimesUntil(x => x > 3, () => n++).ToList();
        Assert.Equal([0, 1, 2, 3], produced);
    }

    [Fact]
    public void TimesUntil_ExcludesTerminatingElement()
    {
        int n = 1;
        var produced = 5.TimesUntil(x => x % 2 == 0, () => n++).ToList();
        Assert.Equal([1], produced);
    }

    [Fact]
    public void TimesUntil_YieldsAll_WhenPredicateNeverTrue()
    {
        int n = 0;
        var produced = 4.TimesUntil(_ => false, () => n++).ToList();
        Assert.Equal([0, 1, 2, 3], produced);
    }

    [Fact]
    public void TimesUntil_IsEager()
    {
        int calls = 0;
        IEnumerable<int> seq = 5.TimesUntil(x => x == 99, () => { calls++; return 1; });
        Assert.Equal(5, calls);
        var list = seq.ToList();
        Assert.Equal(5, calls);
        Assert.Equal(5, list.Count);
    }

    [Fact]
    public void TimesUntil_Zero_YieldsEmpty()
    {
        Assert.Empty(0.TimesUntil(_ => true, () => 1));
    }

    [Fact]
    public void TimesUntil_Negative_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => (-1).TimesUntil(_ => true, () => 1));
    }

    [Fact]
    public void TimesUntil_NullPredicate_Throws()
    {
        Assert.Throws<NullReferenceException>(() => 2.TimesUntil(null!, () => 123));
    }

    [Fact]
    public void TimesUntil_NullFunc_Throws()
    {
        Assert.Throws<NullReferenceException>(() => 2.TimesUntil(_ => false, (Func<int>)null!));
    }
}
