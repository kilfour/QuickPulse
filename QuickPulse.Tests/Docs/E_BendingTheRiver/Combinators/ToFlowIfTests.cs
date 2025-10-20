using QuickPulse.Arteries;
using QuickPulse.Explains;
using QuickPulse.Explains.Formatters;

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
        Assert.Equal(Even_result_values(), BendingTheRiver.GetResult(ToFlowIf_flow(ToFlowIf_SubFlow())));
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
        Assert.Equal(Even_result_values(), BendingTheRiver.GetResult(ToFlowIf_flow_factory()));
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

    // [Fact]
    // [DocHeader("Predicate: Single Value")]
    // [DocContent("Executes when predicate returns true for the current state.")]
    // public void Predicate_Single()
    // {
    //     var collector = TheCollector.Exhibits<int>();
    //     Signal.From<Unit>(
    //             from start in Pulse.Start<Unit>()
    //             from _ in Pulse.Prime(() => 42)
    //             from __ in Pulse.ToFlowIf<int, int>(x => x == 42, TraceFlow(), () => 99)
    //             select start)
    //         .SetArtery(collector)
    //         .Pulse();
    //     Assert.Single(collector.TheExhibit, 99);
    // }

    // [Fact]
    // [DocHeader("Predicate: Collection")]
    // [DocContent("Runs subflow for each value when predicate holds.")]
    // public void Predicate_Collection()
    // {
    //     var collector = TheCollector.Exhibits<int>();
    //     Signal.From<Unit>(
    //             from start in Pulse.Start<Unit>()
    //             from _ in Pulse.Prime(() => 1)
    //             from __ in Pulse.ToFlowIf<int, int>(x => x == 1, TraceFlow(), () => new[] { 10, 11 })
    //             select start)
    //         .SetArtery(collector)
    //         .Pulse();
    //     Assert.Equal(new[] { 10, 11 }, collector.TheExhibit);
    // }

    // [Fact]
    // [DocHeader("False Path")]
    // [DocContent("When flag is false or predicate fails, no subflow executes.")]
    // public void Skipped_When_False()
    // {
    //     var collector = TheCollector.Exhibits<int>();
    //     Signal.From<Unit>(
    //             from start in Pulse.Start<Unit>()
    //             from _ in Pulse.ToFlowIf(false, TraceFlow(), () => 42)
    //             select start)
    //         .SetArtery(collector)
    //         .Pulse();
    //     Assert.Empty(collector.TheExhibit);
    // }
}
