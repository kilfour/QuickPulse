using QuickPulse.Explains;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Docs.D_MakeItFlow;

[DocFile]
[DocContent(
@"> Building larger behaviours from tiny flows.  

QuickPulse is about *composing* small, predictable `Flow<T>` building blocks. 
This chapter shows how to wire those flows together.")]
public class MakeItFlow
{
    [DocHeader("Then")]
    [DocContent(
@"`Then` runs `flow`, discards its value, and continues with `next` in the **same** state.
It's the flow-level equivalent of do this, *then* do that.")]
    [Fact]
    [DocExample(typeof(MakeItFlow), nameof(Then_get_flow))]
    public void Then()
    {
        var holden = TheString.Catcher();
        Signal.From(Then_get_flow())
            .SetArtery(holden)
            .Pulse(42);
        Assert.Equal("... 42", holden.Whispers());
    }

    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<int> Then_get_flow()
    {
        var dot = Pulse.Trace(".");
        var space = Pulse.Trace(" ");
        var flow =
            from input in Pulse.Start<int>()
            from _1 in dot.Then(dot).Then(dot).Then(space) // <=
            from _2 in Pulse.Trace(input)
            select input;
        // Pulse 42 => results in '... 42'.
        return flow;
    }

    [CodeExample]
    private static Flow<int> SubFlow()
    {
        return
            from input in Pulse.Start<int>()
            from _ in Pulse.Trace(input + 1)
            select input;
    }

    [Fact]
    [DocHeader("ToFlow")]
    [DocContent("Given this *sub* flow:")]
    [DocExample(typeof(MakeItFlow), nameof(SubFlow))]
    [DocContent("`Pulse.ToFlow(...)` Executes a subflow over a value.")]
    [DocExample(typeof(MakeItFlow), nameof(Pulse_to_flow))]
    [CodeSnippet]
    public void Pulse_to_flow()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.ToFlow(SubFlow(), input)    // <=
            select input;
        var latch = TheLatch.Holds<int>();
        Signal.From(flow).SetArtery(latch).Pulse(41);
        Assert.Equal(42, latch.Q);
    }

    [Fact]
    [DocContent("An overload exist that allows for executing a subflow over a collection of values.")]
    [DocExample(typeof(MakeItFlow), nameof(Pulse_to_flow_collection))]
    [CodeSnippet]
    public void Pulse_to_flow_collection()
    {
        var flow =
            from input in Pulse.Start<List<int>>()
            from _ in Pulse.ToFlow(SubFlow(), input)    // <=
            select input;
        var collector = TheCollector.Exhibits<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse([41, 41]);
        Assert.Equal([42, 42], collector.TheExhibit);
    }

    [Fact]
    [DocContent("Furthermore both the above methods can be used with a *Flow Factory Method*.")]
    [DocContent("Single value:")]
    [DocExample(typeof(MakeItFlow), nameof(Pulse_to_flow_factory))]
    [CodeSnippet]
    public void Pulse_to_flow_factory()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.ToFlow(a => Pulse.Trace(a + 1), input)    // <=
            select input;
        var latch = TheLatch.Holds<int>();
        var signal = Signal.From(flow).SetArtery(latch);
        signal.Pulse(41);
        Assert.Equal(42, latch.Q);
    }

    [Fact]
    [DocContent("Multiple values:")]
    [DocExample(typeof(MakeItFlow), nameof(Pulse_to_flow_factory_collection))]
    [CodeSnippet]
    public void Pulse_to_flow_factory_collection()
    {
        var flow =
            from input in Pulse.Start<List<int>>()
            from _ in Pulse.ToFlow(a => Pulse.Trace(a + 1), input)    // <=
            select input;
        var collector = TheCollector.Exhibits<int>();
        var signal = Signal.From(flow).SetArtery(collector);
        signal.Pulse([41, 41]);
        Assert.Equal([42, 42], collector.TheExhibit);
    }
}