using QuickPulse.Explains;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace QuickPulse.Tests.Docs.C_MemoryAndManipulation;

[DocFile]
[DocContent("> How QuickPulse remembers, updates, and temporarily alters state.")]
[DocExample(typeof(MemoryAndManipulation), nameof(Scoped_with_Manipulate))]
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
    [DocContent("The `Draw<TBox, T>(Func<TBox, T> func)` is just a bit of sugar to enable accessing nested values.")]
    public void Draw_reads_current_value_project()
    {
        var flow =
            from start in Pulse.Start<Unit>()
            from _ in Pulse.Prime(() => new Box<int>(42))
            from value in Pulse.Draw<Box<int>, int>(a => a.Value)
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
    [DocContent("The return value of `Manipulate` is the **new value**, which can be used immediately in the flow.")]
    [DocExample(typeof(MemoryAndManipulation), nameof(Manipulate_return_value))]
    [CodeSnippet]
    public void Manipulate_return_value()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 0)
            from i in Pulse.Manipulate<int>(x => x + 10) // <= update int cell
            from _2 in Pulse.Trace(i + input)            // <= use the new value
            select input;
        var latch = TheLatch.Holds<int>();
        Signal.From(flow).SetArtery(latch).Pulse(32);
        Assert.Equal(42, latch.Q);
    }

    [Fact]
    [DocHeader("Scoped: temporary overrides with automatic restore.")]
    [DocContent("`Scoped<T>(enter, innerFlow)` runs `innerFlow` with a **temporary** value for the gathered cell of type `T`. On exit, the outer value is restored.")]
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
    [CodeSnippet]
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



    [Fact]
    [DocHeader("Type Identity Matters")]
    [DocContent("Use wrapper records to keep multiple cells of the same underlying type.  ")]
    [DocExample(typeof(Int1))]
    [DocExample(typeof(Int2))]
    [DocExample(typeof(MemoryAndManipulation), nameof(Type_matters_using_records_flow))]
    public void Type_matters_using_records()
    {
        var latch = TheLatch.Holds<int>();
        Signal.From(Type_matters_using_records_flow()).SetArtery(latch).Pulse();
        Assert.Equal(3, latch.Q);
    }

    [CodeExample]
    public record Int1(int Number) { }
    [CodeExample]
    public record Int2(int Number) { }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<Unit> Type_matters_using_records_flow()
    {
        return from start in Pulse.Start<Unit>()
               from _1 in Pulse.Prime(() => new Int1(1))
               from _2 in Pulse.Prime(() => new Int2(2))
               from _3 in Pulse.Trace(_1.Number + _2.Number)
               select start;
    }

    [Fact]
    [DocHeader("Postfix Operators")]
    [DocContent(
@"Although the behaviour is logical once you think about it, it can feel a bit unintuitive,
but when using Postfix operators, beware that they return the *old* value.")]
    [DocExample(typeof(MemoryAndManipulation), nameof(Manipulate_postfix_operators))]
    [CodeSnippet]
    [DocContent(
@"Use prefix form or pure expressions instead.  
* **Recommended:** `Pulse.Manipulate<int>(a => a + 1)`")]
    public void Manipulate_postfix_operators()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Prime(() => 0)
            from __ in Pulse.Manipulate<int>(a => a++) // <= int is still 0 in memory cell
            from now in Pulse.Trace<int>(a => a + input)
            select input;
        var latch = TheLatch.Holds<int>();
        Signal.From(flow).SetArtery(latch).Pulse(42);
        Assert.Equal(42, latch.Q);
    }
}