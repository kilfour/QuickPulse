using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.B_OneSignalOneState.SignalReference;

[DocFile]
[DocContent(
@"**`Signal.From(...)`** is a simple factory method used to get hold of a `Signal<T>` instance
that wraps the passed in `Flow<T>`.")]
public class A_From
{
    [Fact]
    [DocExample(typeof(A_From), nameof(Signal_from_example))]
    public void Signal_from()
        => Assert.IsType<Signal<int>>(Signal_from_example());

    [CodeSnippet]
    [CodeReplace("return signal;", "")]
    private static Signal<int> Signal_from_example()
    {
        var flow =
            from anInt in Pulse.Start<int>()
            select anInt;
        var signal = Signal.From(flow);
        return signal;
    }

    [Fact]
    [DocContent("**`Signal.From<T>(Func<T, Flow<Unit>>`** is a useful overload that allows for inlining simple flows upon Signal creation.")]
    [DocExample(typeof(A_From), nameof(Signal_from_factory_example))]
    public void Signal_from_factory()
        => Assert.IsType<Signal<int>>(Signal_from_factory_example());

    [CodeSnippet]
    [CodeReplace("return signal;", "")]
    private static Signal<int> Signal_from_factory_example()
    {
        var signal = Signal.From<int>(a => Pulse.Trace(a));
        return signal;
    }
}