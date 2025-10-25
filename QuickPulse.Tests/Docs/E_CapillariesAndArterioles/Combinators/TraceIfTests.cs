using QuickPulse.Explains;
using QuickPulse.Explains.Formatters;
using QuickPulse.Tests.Docs.E_OnCapillariesAndArterioles;

namespace QuickPulse.Tests.Docs.E_BendingTheRiver.Combinators;


[DocFileHeader("TraceIf")]
public class TraceIfTests
{
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
    [DocExample(typeof(TraceIfTests), nameof(TraceIf_flow))]
    public void TraceIf()
    {
        Assert.Equal(CapillariesAndArterioles.Default_result_values(), CapillariesAndArterioles.GetResult(TraceIf_flow()));
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
    [DocExample(typeof(TraceIfTests), nameof(TraceIf_flow_state))]
    public void TraceIf_state()
    {
        Assert.Equal(CapillariesAndArterioles.Default_result_values(), CapillariesAndArterioles.GetResult(TraceIf_flow_state()));
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
    [DocExample(typeof(TraceIfTests), nameof(TraceIf_flow_state_twice))]
    [DocContent("**Result:**")]
    [DocExample(typeof(TraceIfTests), nameof(TraceIf_state_twice_result), "bash")]
    public void TraceIf_state_twice()
    {
        Assert.Equal(TraceIf_state_twice_result(), CapillariesAndArterioles.GetResult(TraceIf_flow_state_twice()));
    }
}