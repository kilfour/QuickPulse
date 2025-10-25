using QuickPulse.Explains;
using QuickPulse.Explains.Formatters;
using QuickPulse.Tests.Docs.E_OnCapillariesAndArterioles;

namespace QuickPulse.Tests.Docs.E_BendingTheRiver.Combinators;


[DocFileHeader("Using a Ternary Conditional Operator (*If/Then/Else*)")]
public class UsingConditionalTernaryOperator
{
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
    [DocExample(typeof(UsingConditionalTernaryOperator), nameof(TernaryConditionalOperator_flow))]
    [DocContent("**Result:**")]
    [DocExample(typeof(UsingConditionalTernaryOperator), nameof(TernaryConditionalOperator_result), "bash")]
    public void TernaryConditionalOperator()
    {
        Assert.Equal(TernaryConditionalOperator_result(), CapillariesAndArterioles.GetResult(TernaryConditionalOperator_flow()));
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
    [DocExample(typeof(UsingConditionalTernaryOperator), nameof(TernaryConditionalOperator_flow_noop))]
    [DocContent("*Note:* While the ternary operator works, QuickPulse provides more idiomatic ways to deal with conditional statemens, which we will look at below.")]
    public void TernaryConditionalOperator_noop()
    {
        Assert.Equal(CapillariesAndArterioles.Default_result_values(), CapillariesAndArterioles.GetResult(TernaryConditionalOperator_flow_noop()));
    }

}