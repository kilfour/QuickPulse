using QuickPulse.Arteries;
using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.E_BendingTheRiver.Combinators;

[DocFileHeader("ToFlowIf")]
public class ToFlowIfTests
{
    [CodeSnippet]
    [CodeRemove("return")]
    [CodeRemove(";")]
    public static string[] Even_result_values()
    {
        return ["2", "4"];
    }

    [CodeSnippet]
    [CodeRemove("return")]
    [CodeRemove(";")]
    public static string[] All_result_values()
    {
        return ["1", "2", "3", "4", "5"];
    }

    [DocContent("All examples in this section use the following *sub* flow definition:")]
    [DocExample(typeof(ToFlowIfTests), nameof(ToFlowIf_SubFlow))]
    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> ToFlowIf_SubFlow()
    {
        return
            from input in Pulse.Start<int>()
            from _ in Pulse.Trace(input.ToString())
            select input;
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> ToFlowIf_flow(Flow<int> subFlow)
    {
        return
            from input in Pulse.Start<int>()
            from _ in Pulse.ToFlowIf(input % 2 == 0, subFlow, () => input)
            select input;
    }

    [Fact]
    [DocContent(
@"The `ToFlowIf` family conditionally executes a subflow.

Use it to embed optional or context-sensitive subflows declaratively.")]
    [DocExample(typeof(ToFlowIfTests), nameof(ToFlowIf_flow))]
    [DocContent("Results in:")]
    [DocExample(typeof(ToFlowIfTests), nameof(Even_result_values))]
    public void ToFlowIf()
    {
        //Assert.Equal(Even_result_values(), CapillariesAndArterioles.GetResult(ToFlowIf_flow(ToFlowIf_SubFlow())));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<List<int>> ToFlowIf_flow_collection(Flow<int> subFlow)
    {
        return
            from input in Pulse.Start<List<int>>()
            from _ in Pulse.ToFlowIf(input.Count == 5, subFlow, () => input)
            select input;
    }

    [CodeSnippet]
    [CodeRemove("return ")]
    private static List<string> RunCollectionSignal(Flow<List<int>> flow)
    {
        return Signal.From(flow)
            .SetArtery(TheCollector.Exhibits<string>())
            .Pulse([42, 43])
            .Pulse([1, 2, 3, 4, 5])
            .Pulse([42, 43, 44])
            .GetArtery<Collector<string>>()
            .TheExhibit;
    }

    [Fact]
    [DocContent("An overload exist that allows for executing a subflow over a collection of values.")]
    [DocContent("Given the following `Signal`:")]
    [DocExample(typeof(ToFlowIfTests), nameof(RunCollectionSignal))]
    [DocContent("And this `Flow`:")]
    [DocExample(typeof(ToFlowIfTests), nameof(ToFlowIf_flow_collection))]
    [DocContent("Results in:")]
    [DocExample(typeof(ToFlowIfTests), nameof(All_result_values))]
    public void ToFlowIf_Collection()
    {
        Assert.Equal(All_result_values(), RunCollectionSignal(ToFlowIf_flow_collection(ToFlowIf_SubFlow())));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> ToFlowIf_flow_factory()
    {
        return
            from input in Pulse.Start<int>()
            from _ in Pulse.ToFlowIf(input % 2 == 0, a => Pulse.Trace(a.ToString()), () => input)
            select input;
    }

    [Fact]
    [DocContent("Both the above methods can be used with a *Flow Factory Method*.")]
    [DocContent("Single value:")]
    [DocExample(typeof(ToFlowIfTests), nameof(ToFlowIf_flow_factory))]
    [CodeSnippet]
    public void Pulse_to_flowif_factory()
    {
        //Assert.Equal(Even_result_values(), CapillariesAndArterioles.GetResult(ToFlowIf_flow_factory()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<List<int>> ToFlowIf_flow_collection_factory()
    {
        return
            from input in Pulse.Start<List<int>>()
            from _ in Pulse.ToFlowIf(input.Count == 5, a => Pulse.Trace(((int)a).ToString()), () => input)
            select input;
    }

    [Fact]
    [DocContent("Multiple values:")]
    [DocExample(typeof(ToFlowIfTests), nameof(ToFlowIf_flow_collection_factory))]
    [CodeSnippet]
    public void Pulse_to_flowIf_factory_collection()
    {
        Assert.Equal(All_result_values(), RunCollectionSignal(ToFlowIf_flow_collection_factory()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> ToFlowIf_flow_using_state(Flow<int> subFlow)
    {
        return
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 2)
            from _2 in Pulse.ToFlowIf<int, int>(a => input % a == 0, subFlow, () => input)
            select input;
    }

    [Fact]
    [DocHeader("Using State", 1)]
    [DocExample(typeof(ToFlowIfTests), nameof(ToFlowIf_flow_using_state))]
    [CodeSnippet]
    public void Pulse_to_flowif_using_state()
    {
        //Assert.Equal(Even_result_values(), CapillariesAndArterioles.GetResult(ToFlowIf_flow_using_state(ToFlowIf_SubFlow())));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<List<int>> ToFlowIf_flow_using_state_collection(Flow<int> subFlow)
    {
        return
            from input in Pulse.Start<List<int>>()
            from _1 in Pulse.Prime(() => 5)
            from _2 in Pulse.ToFlowIf<int, int>(a => input.Count == a, subFlow, () => input)
            select input;
    }

    [Fact]
    [DocHeader("Using State: Collection.", 1)]
    [DocExample(typeof(ToFlowIfTests), nameof(ToFlowIf_flow_using_state_collection))]
    [CodeSnippet]
    public void Pulse_to_flowif_using_state_collection()
    {
        Assert.Equal(All_result_values(), RunCollectionSignal(ToFlowIf_flow_using_state_collection(ToFlowIf_SubFlow())));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<int> ToFlowIf_flow_using_state_factory()
    {
        return
            from input in Pulse.Start<int>()
            from _1 in Pulse.Prime(() => 2)
            from _2 in Pulse.ToFlowIf<int, int>(a => input % a == 0, a => Pulse.Trace(a.ToString()), () => input)
            select input;
    }

    [Fact]
    [DocHeader("Using State: Factory", 1)]
    [DocExample(typeof(ToFlowIfTests), nameof(ToFlowIf_flow_using_state_factory))]
    [CodeSnippet]
    public void Pulse_to_flowif_using_state_factory()
    {
        //Assert.Equal(Even_result_values(), CapillariesAndArterioles.GetResult(ToFlowIf_flow_using_state_factory()));
    }

    [CodeSnippet]
    [CodeRemove("return")]
    private static Flow<List<int>> ToFlowIf_flow_using_state_factor_collection()
    {
        return
            from input in Pulse.Start<List<int>>()
            from _1 in Pulse.Prime(() => 5)
            from _2 in Pulse.ToFlowIf<int, int>(a => input.Count == a, a => Pulse.Trace(a.ToString()), () => input)
            select input;
    }

    [Fact]
    [DocHeader("Using State: Factory", 1)]
    [DocExample(typeof(ToFlowIfTests), nameof(ToFlowIf_flow_using_state_factor_collection))]
    [CodeSnippet]
    public void Pulse_to_flowif_using_state_factory_collection()
    {
        Assert.Equal(All_result_values(), RunCollectionSignal(ToFlowIf_flow_using_state_factor_collection()));
    }
}
