using QuickPulse.Explains;
using QuickPulse.Arteries;


namespace QuickPulse.Tests.Docs.A_AQuickPulse;


[DocFile]
[DocContent("> How QuickPulse remembers, updates, and temporarily alters state.")]
public class MemoryAndManipulation
{
    [Fact]
    [DocHeader("Prime: one-time lazy initialization.")]
    [DocContent("`Prime(() => T)` computes and stores a value **once per signal lifetime**.")]
    public void Prime_runs_once_per_signal()
    {
        var created = 0;
        var flow =
            from first in Pulse.Prime(() => { created++; return true; })
            from _1 in Pulse.Trace(first)
            from _2 in Pulse.Manipulate<bool>(_ => false)
            select Unit.Instance;
        var signal = Signal.From(flow).SetArtery(TheCollector.Exhibits<bool>());
        signal.Pulse([Unit.Instance, Unit.Instance]);
        Assert.Equal(1, created);
    }

    [Fact]
    [DocHeader("Draw: read from memory.")]
    [DocContent("`Draw<T>()` retrieves the current value from the signal's memory for type `T`.")]
    public void Draw_reads_current_value()
    {
        var flow =
            from start in Pulse.Start<Unit>()
            from _ in Pulse.Prime(() => 42)
            from value in Pulse.Draw<int>()
            from __ in Pulse.Trace(value)
            select start;
        var latch = TheLatch.Holds<int>();
        Signal.From(flow).SetArtery(latch).Pulse(Unit.Instance);
        Assert.Equal(42, latch.Q);
    }

    [Fact]
    [DocHeader("Manipulate: controlled mutation of *primed* state.")]
    [DocContent("`Manipulate<T>(Func<T,T>)` updates the current value of the gathered cell for type `T`.")]
    public void Manipulate_updates_gathered_state()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Gather(0)
            from __ in Pulse.Manipulate<int>(x => x + 10)
            from now in Pulse.Trace<int>(a => a + input)
            select input;
        var latch = TheLatch.Holds<int>();
        Signal.From(flow).SetArtery(latch).Pulse(32);
        Assert.Equal(42, latch.Q);
    }

    [Fact]
    [DocHeader("")]
    [DocContent("")]
    public void Foo()
    {

    }

    // [Fact]
    // [DocHeader("")]
    // [DocContent("")]
    // public void Foo()
    // {

    // }
}