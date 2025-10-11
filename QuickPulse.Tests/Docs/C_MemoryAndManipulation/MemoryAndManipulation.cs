using QuickPulse.Explains;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Docs.C_MemoryAndManipulation;

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
            from _ in Pulse.Prime(() => 0)
            from __ in Pulse.Manipulate<int>(x => x + 10)
            from now in Pulse.Trace<int>(a => a + input)
            select input;
        var latch = TheLatch.Holds<int>();
        Signal.From(flow).SetArtery(latch).Pulse(32);
        Assert.Equal(42, latch.Q);
    }

    [Fact]
    [DocHeader("Scoped: temporary overrides with automatic restore.")]
    [DocContent("`Scoped<T>(enter, innerFlow)` runs `innerFlow` with a **temporary** value for the gathered cell of type `T`. On exit, the outer value is restored.")]
    [DocContent("Any `Manipulate<T>` inside the scope affects the **scoped** value and is discarded on exit.")]
    public void Scoped_applies_temp_value_and_restores()
    {
        var seen = TheCollector.Exhibits<string>();
        var flow =
            from input in Pulse.Start<Unit>()
            from _1 in Pulse.Prime(() => 1)
            from _2 in Pulse.Trace<int>(a => $"outer: {a}")
            from _3 in Pulse.Scoped<int>(a => a + 1, Pulse.Trace<int>(a => $"inner: {a}"))
            from _4 in Pulse.Trace<int>(a => $"restored: {a}")
            select Unit.Instance;
        Signal.From(flow).SetArtery(seen).Pulse(Unit.Instance);
        Assert.Equal(new object[] { "outer: 1", "inner: 2", "restored: 1" }, seen.TheExhibit);
    }

    [Fact]
    [DocContent("Any `Manipulate<T>` inside the scope affects the **scoped** value and is discarded on exit.")]
    public void Scoped_with_Manipulate()
    {
        var seen = TheCollector.Exhibits<string>();
        var flow =
            from input in Pulse.Start<Unit>()
            from _1 in Pulse.Prime(() => 1)
            from _2 in Pulse.Trace<int>(a => $"outer: {a}")
            from _3 in Pulse.Scoped<int>(a => a + 1,
                from __1 in Pulse.Trace<int>(a => $"inner: {a}")
                from __2 in Pulse.Manipulate<int>(a => a + 1)
                from __3 in Pulse.Trace<int>(a => $"inner manipulated: {a}")
                select Unit.Instance)
            from _4 in Pulse.Trace<int>(a => $"restored: {a}")
            select Unit.Instance;
        Signal.From(flow).SetArtery(seen).Pulse(Unit.Instance);
        Assert.Equal(new object[] { "outer: 1", "inner: 2", "inner manipulated: 3", "restored: 1" }, seen.TheExhibit);
    }


    public record Int1(int Number);
    public record Int2(int Number);
    [Fact]
    [DocContent("* **Type identity matters**: Use wrapper records to keep multiple cells of the same underlying type.")]
    public void Type_matters_using_records()
    {
        var flow =
            from start in Pulse.Start<Unit>()
            from _1 in Pulse.Prime(() => new Int1(1))
            from _2 in Pulse.Prime(() => new Int2(2))
            from _3 in Pulse.Trace(_1.Number + _2.Number)
            select start;
        var latch = TheLatch.Holds<int>();
        Signal.From(flow).SetArtery(latch).Pulse();
        Assert.Equal(3, latch.Q);
    }
}