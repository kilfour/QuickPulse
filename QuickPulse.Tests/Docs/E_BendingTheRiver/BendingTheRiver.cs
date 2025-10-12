using QuickPulse.Arteries;
using QuickPulse.Explains;
using QuickPulse.Explains.Formatters;

namespace QuickPulse.Tests.Docs.E_BendingTheRiver;

[DocFile]
[DocContent(
@"> About knees and meanders.

This chapter explores and explains the various ways you can control the execution flow of an ... erm ... `Flow`.  
It is more of a reference chapter and more prone to changes, than any of the other chapters.
This area is still evolving, expect simplifications.

All flow examples below will be executed using the following `Signal`:")]
[DocExample(typeof(BendingTheRiver), nameof(GetResult))]
[DocContent("In addition, unless explicitly specified the result of executing the example flow will always be: ")]
[DocExample(typeof(BendingTheRiver), nameof(Default_result_values), "bash")]
public class BendingTheRiver
{
    [CodeSnippet]
    [CodeRemove("return ")]
    private static IEnumerable<string> GetResult(Flow<int> flow)
    {
        return Signal.From(flow)
            .SetArtery(TheCollector.Exhibits<string>())
            .Pulse([1, 2, 3, 4, 5])
            .GetArtery<Collector<string>>()
            .TheExhibit;
    }

    [CodeSnippet]
    [CodeFormat(typeof(StringArrayToString))]
    private static string[] Default_result_values()
    {
        return ["even", "even"];
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> TernaryConditionalOperator_flow()
    {
        return
            from input in Pulse.Start<int>()
            let conditional =
                input % 2 == 0
                ? Pulse.Trace("even")
                : Pulse.Trace("uneven")
            from _ in conditional
            select input;
    }

    [CodeSnippet]
    [CodeFormat(typeof(StringArrayToString))]
    private static string[] TernaryConditionalOperator_result()
    {
        return ["uneven", "even", "uneven", "even", "uneven"];
    }

    [Fact]
    [DocHeader("Using a Ternary Conditional Operator (*If/Then/Else*)")]
    [DocExample(typeof(BendingTheRiver), nameof(TernaryConditionalOperator_flow))]
    [DocContent("**Result:**")]
    [DocExample(typeof(BendingTheRiver), nameof(TernaryConditionalOperator_result), "bash")]
    public void TernaryConditionalOperator()
    {
        Assert.Equal(TernaryConditionalOperator_result(), GetResult(TernaryConditionalOperator_flow()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> TernaryConditionalOperator_flow_noop()
    {
        return
            from input in Pulse.Start<int>()
            let conditional =
                input % 2 == 0
                ? Pulse.Trace("even")
                : Pulse.NoOp()
            from _ in conditional
            select input;
    }

    [Fact]
    [DocContent("> Prefer Pulse.NoOp() when you want an If/Then without an else-branch:")]
    [DocExample(typeof(BendingTheRiver), nameof(TernaryConditionalOperator_flow_noop))]
    [DocContent("*Note:* While the ternary operator works, QuickPulse provides more idiomatic ways to deal with conditional statemens, which we will look at below.")]
    public void TernaryConditionalOperator_noop()
    {
        Assert.Equal(Default_result_values(), GetResult(TernaryConditionalOperator_flow_noop()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> When_flow()
    {
        return
            from input in Pulse.Start<int>()
            from _ in Pulse.When(input % 2 == 0, Pulse.Trace("even"))
            select input;
    }


    [Fact]
    [DocHeader("When")]
    [DocExample(typeof(BendingTheRiver), nameof(When_flow))]
    public void When()
    {
        Assert.Equal(Default_result_values(), GetResult(When_flow()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> When_flow_factory()
    {
        return
            from input in Pulse.Start<int>()
            from _ in Pulse.When(input % 2 == 0, () => Pulse.Trace("even"))
            select input;
    }

    [Fact]
    [DocHeader("Flow Factory Version", 1)]
    [DocExample(typeof(BendingTheRiver), nameof(When_flow_factory))]
    public void When_factory()
    {
        Assert.Equal(Default_result_values(), GetResult(When_flow_factory()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> When_flow_state()
    {
        return
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 2)
            from _2 in Pulse.When<int>(a => input % a == 0, Pulse.Trace("even"))
            select input;
    }

    [Fact]
    [DocHeader("Using State", 1)]
    [DocExample(typeof(BendingTheRiver), nameof(When_flow_state))]
    public void When_state()
    {
        Assert.Equal(Default_result_values(), GetResult(When_flow_state()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> When_flow_state_factory()
    {
        return
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 2)
            from _2 in Pulse.When<int>(a => input % a == 0, () => Pulse.Trace("even"))
            select input;
    }

    [Fact]
    [DocHeader("Using State: Flow Factory Version", 1)]
    [DocExample(typeof(BendingTheRiver), nameof(When_flow_state_factory))]
    [DocContent(
@"> **Why the factory overloads?**  
> `When(..., () => flow)` and many other `Pulse` methods using the factory flow pattern defer creating the subflow/value until the condition is true, avoiding work and allocations on the false path.")]
    public void When_state_factory()
    {
        Assert.Equal(Default_result_values(), GetResult(When_flow_state_factory()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> TraceIf_flow()
    {
        return
            from input in Pulse.Start<int>()
            from _ in Pulse.TraceIf(input % 2 == 0, () => "even")
            select input;
    }

    [Fact]
    [DocHeader("TraceIf")]
    [DocExample(typeof(BendingTheRiver), nameof(TraceIf_flow))]
    public void TraceIf()
    {
        Assert.Equal(Default_result_values(), GetResult(TraceIf_flow()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> TraceIf_flow_state()
    {
        return
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 2)
            from _2 in Pulse.TraceIf<int>(a => input % a == 0, () => "even")
            select input;
    }

    [Fact]
    [DocHeader("Using State", 1)]
    [DocExample(typeof(BendingTheRiver), nameof(TraceIf_flow_state))]
    public void TraceIf_state()
    {
        Assert.Equal(Default_result_values(), GetResult(TraceIf_flow_state()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> TraceIf_flow_state_twice()
    {
        return
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 2)
            from _2 in Pulse.TraceIf<int>(a => input % a == 0, a => $"{a}:even")
            select input;
    }

    [CodeSnippet]
    [CodeFormat(typeof(StringArrayToString))]
    private static string[] TraceIf_state_twice_result()
    {
        return ["2:even", "2:even"];
    }

    [Fact]
    [DocHeader("Using State, ... Twice", 1)]
    [DocExample(typeof(BendingTheRiver), nameof(TraceIf_flow_state_twice))]
    [DocContent("**Result:**")]
    [DocExample(typeof(BendingTheRiver), nameof(TraceIf_state_twice_result), "bash")]
    public void TraceIf_state_twice()
    {
        Assert.Equal(TraceIf_state_twice_result(), GetResult(TraceIf_flow_state_twice()));
    }
}