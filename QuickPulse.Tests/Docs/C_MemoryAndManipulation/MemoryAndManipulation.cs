using QuickPulse.Explains;
using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Tests.Tools;

namespace QuickPulse.Tests.Docs.C_MemoryAndManipulation;

[DocFile]
[DocContent(
@"> How QuickPulse remembers, updates, and temporarily alters state.

Each signal maintains **gathered cells** (keyed by *type identity*), think of them as the signal's **internal organs**
that store and process specific data types.
Just as your heart handles blood and lungs handle air, each gathered cell specializes in a particular data type.
")]
[DocExample(typeof(MemoryAndManipulation), nameof(Scoped_with_Manipulate_example))]
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
            select Flow.Continue;
        var signal = Signal.From(flow).SetArtery(TheCollector.Exhibits<bool>());
        signal.Pulse([Flow.Continue, Flow.Continue]);
        Assert.Equal(1, created);
    }

    [Fact]
    [DocHeader("Draw: read from memory.")]
    [DocContent("`Draw<T>()` retrieves the current value from the signal's memory for type `T`.")]
    public void Draw_reads_current_value()
    {
        var flow =
            from _ in Pulse.Start<Flow>()
            from __ in Pulse.Prime(() => 42)
            from value in Pulse.Draw<int>()
            from ___ in Pulse.Trace(value)
            select Flow.Continue;
        var latch = TheLatch.Holds<int>();
        Signal.From(flow).SetArtery(latch).Pulse(Flow.Continue);
        Assert.Equal(42, latch.Q);
    }

    [Fact]
    [DocContent("The `Draw<TCell, T>(Func<TCell, T> func)` is just a bit of sugar to enable accessing nested values.")]
    public void Draw_reads_current_value_project()
    {
        var flow =
            from _ in Pulse.Start<Flow>()
            from __ in Pulse.Prime(() => new Cell<int>(42))
            from value in Pulse.Draw<Cell<int>, int>(a => a.Value)
            from ___ in Pulse.Trace(value)
            select Flow.Continue;
        var latch = TheLatch.Holds<int>();
        Signal.From(flow).SetArtery(latch).Pulse(Flow.Continue);
        Assert.Equal(42, latch.Q);
    }

    [Fact]
    [DocHeader("State aware overloads.")]
    [DocContent(
@"Most `Pulse` methods have one or more utility overloads that combines `.Draw()` functionality
with the overloaded method's functionality.  
It can be seen in the example at the top, but here's another one, showing a more focused usage:")]
    [DocExample(typeof(MemoryAndManipulation), nameof(Pulse_State_overloads_example))]
    public void Pulse_State_overloads()
    {
        var latch = TheLatch.Holds<int>();
        Signal.From(Pulse_State_overloads_example()).SetArtery(latch).Pulse(Flow.Continue);
        Assert.Equal(42, latch.Q);
    }

    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<Flow> Pulse_State_overloads_example()
    {
        var flow =
            from _ in Pulse.Start<Flow>()
            from __ in Pulse.Prime(() => 41)
            from ___ in Pulse.Trace<int>(a => a + 1)
            select Flow.Continue;
        // Pulse() => results in 42
        return flow;
    }

    [Fact]
    [DocHeader("Manipulate: controlled mutation of *primed* state.")]
    [DocContent("`Manipulate<T>(Func<T,T>)` updates the current value of the *gathered cell* for type `T`.")]
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
    [DocExample(typeof(MemoryAndManipulation), nameof(Manipulate_return_value_example))]
    public void Manipulate_return_value()
    {
        var latch = TheLatch.Holds<int>();
        Manipulate_return_value_example(latch);
        Assert.Equal(42, latch.Q);
    }

    [CodeSnippet]
    [CodeRemove(".SetArtery(latch)")]
    private static void Manipulate_return_value_example(Latch<int> latch)
    {
        var flow =
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 0)
            from i in Pulse.Manipulate<int>(x => x + 10) // <= update int cell
            from _2 in Pulse.Trace(i + input)            // <= use the new value
            select input;
        Signal.From(flow).SetArtery(latch).Pulse(32);
    }

    [Fact]
    [DocHeader("Scoped: temporary overrides with automatic restore.")]
    [DocContent("`Scoped<T>(enter, innerFlow)` runs `innerFlow` with a **temporary** value for the *gathered cell* of type `T`. On exit, the outer value is restored.")]
    public void Scoped_applies_temp_value_and_restores()
    {
        var seen = TheCollector.Exhibits<string>();
        var flow =
            from _ in Pulse.Start<Flow>()
            from _1 in Pulse.Prime(() => 1)
            from _2 in Pulse.Trace<int>(a => $"outer: {a}")
            from _3 in Pulse.Scoped<int>(a => a + 1, Pulse.Trace<int>(a => $"inner: {a}"))
            from _4 in Pulse.Trace<int>(a => $"restored: {a}")
            select Flow.Continue;
        Signal.From(flow).SetArtery(seen).Pulse(Flow.Continue);
        Assert.Equal(new object[] { "outer: 1", "inner: 2", "restored: 1" }, seen.TheExhibit);
    }

    [Fact]
    [DocContent("Any `Manipulate<T>` inside the scope affects the **scoped** value and is discarded on exit.")]
    public void Scoped_with_Manipulate()
    {
        var collector = TheCollector.Exhibits<string>();
        Scoped_with_Manipulate_example(collector);
        Assert.Equal(new object[] { "outer: 1", "inner: 2", "inner manipulated: 3", "restored: 1" }, collector.TheExhibit);
    }

    [CodeSnippet]
    [CodeRemove(".SetArtery(collector)")]
    private static void Scoped_with_Manipulate_example(Collector<string> collector)
    {
        var flow =
            from _ in Pulse.Start<Flow>()
            from _1 in Pulse.Prime(() => 1)
            from _2 in Pulse.Trace<int>(a => $"outer: {a}")
            from _3 in Pulse.Scoped<int>(a => a + 1,
                from __1 in Pulse.Trace<int>(a => $"inner: {a}")
                from __2 in Pulse.Manipulate<int>(a => a + 1)
                from __3 in Pulse.Trace<int>(a => $"inner manipulated: {a}")
                select Flow.Continue)
            from _4 in Pulse.Trace<int>(a => $"restored: {a}")
            select Flow.Continue;
        Signal.From(flow).SetArtery(collector).Pulse(Flow.Continue);
        // Results in => 
        //     [ "outer: 1", "inner: 2", "inner manipulated: 3", "restored: 1" ]
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
    private static Flow<Flow> Type_matters_using_records_flow()
    {
        return from _ in Pulse.Start<Flow>()
               from _1 in Pulse.Prime(() => new Int1(1))
               from _2 in Pulse.Prime(() => new Int2(2))
               from _3 in Pulse.Trace(_1.Number + _2.Number)
               select Flow.Continue;
    }

    [Fact]
    [DocHeader("Postfix Operators")]
    [DocContent(
@"Although the behaviour is logical once you think about it, it can feel a bit unintuitive,
but when using Postfix operators, beware that they return the *old* value.")]
    [DocExample(typeof(MemoryAndManipulation), nameof(Manipulate_postfix_operators_example))]
    [DocContent(
@"Use prefix form or pure expressions instead.  
* **Recommended:** `Pulse.Manipulate<int>(a => a + 1)`")]
    public void Manipulate_postfix_operators()
    {
        var latch = TheLatch.Holds<int>();
        Manipulate_postfix_operators_example(latch);
        Assert.Equal(41, latch.Q);
    }

    [CodeSnippet]
    [CodeRemove(".SetArtery(latch)")]
    private static void Manipulate_postfix_operators_example(Latch<int> latch)
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Prime(() => 0)
            from __ in Pulse.Manipulate<int>(a => a++) // <= int is still 0 in memory cell
            from now in Pulse.Trace<int>(a => a + input)
            select input;
        Signal.From(flow).SetArtery(latch).Pulse(41);
        // Result => 41. Not 42!
    }
}