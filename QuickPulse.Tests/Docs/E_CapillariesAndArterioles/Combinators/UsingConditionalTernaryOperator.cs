using QuickPulse.Arteries;
using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.E_CapillariesAndArterioles.Combinators;


[DocFileHeader("Using a Ternary Conditional Operator (*If/Then/Else*)")]
public class UsingConditionalTernaryOperator
{
    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<int> TernaryConditionalOperator_flow()
    {
        var flow =
            from input in Pulse.Start<int>()
            let conditional =
                input % 2 == 0
                ? Pulse.Trace("even")
                : Pulse.Trace("uneven")
            from _ in conditional
            select input;
        // Pulse [1, 2, 3, 4, 5] => results in ["uneven", "even", "uneven", "even", "uneven"].
        return flow;
    }

    [Fact]
    [DocExample(typeof(UsingConditionalTernaryOperator), nameof(TernaryConditionalOperator_flow))]
    public void TernaryConditionalOperator()
    {
        var collector = Collect.ValuesOf<string>();
        Signal.From(TernaryConditionalOperator_flow())
            .SetArtery(collector)
            .Pulse([1, 2, 3, 4, 5]);
        Assert.Equal(["uneven", "even", "uneven", "even", "uneven"], collector.Values);
    }

    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<int> TernaryConditionalOperator_flow_noop()
    {
        var flow =
            from input in Pulse.Start<int>()
            let conditional =
                input % 2 == 0
                ? Pulse.Trace("even")
                : Pulse.NoOp()
            from _ in conditional
            select input;
        // Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
        return flow;
    }

    [Fact]
    [DocContent("Prefer `Pulse.NoOp()` when you want an if/then without an else-branch:")]
    [DocExample(typeof(UsingConditionalTernaryOperator), nameof(TernaryConditionalOperator_flow_noop))]
    [DocContent("*Note:* While the ternary operator works, QuickPulse provides more idiomatic ways to deal with conditional statemens, which we will look at below.")]
    public void TernaryConditionalOperator_noop()
    {
        var collector = Collect.ValuesOf<string>();
        Signal.From(TernaryConditionalOperator_flow_noop())
            .SetArtery(collector)
            .Pulse([1, 2, 3, 4, 5]);
        Assert.Equal(["even", "even"], collector.Values);
    }
}