using QuickPulse.Explains;
using QuickPulse.Arteries;
using QuickPulse.Tests.Tools;

namespace QuickPulse.Tests.Docs.D_Circulation;

[DocFile]
[DocContent(
@"> Make it flow, number one.  

While it is entirely possible, and sometimes weirdly intellectually satisfying,
to write an entire QuickPulse Flow as one big LINQ expression,
it would be silly to ignore one of the main strengths of the LINQy approach: Composability.

QuickPulse provides two main ways to achieve this.")]
public class Circulation
{
    [DocHeader("Then")]
    [DocContent(
@"The `Then` combinator joins two flows sequentially while sharing the same internal state.
It's the flow-level equivalent of saying *do this, then that*.")]
    [Fact]
    [DocExample(typeof(Circulation), nameof(Then_get_flow))]
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

    [Fact]
    [DocHeader("ToFlow")]
    [DocContent("If `Then` is about sequence, `ToFlow` is about delegation. It executes another flow *as part* of the current one.")]
    [DocExample(typeof(Circulation), nameof(ToFlow_get_flow))]
    [DocContent(
@"This lets you reuse a named or shared flow inside another.
The subflow inherits the same signal state, so memory cells and arteries are visible across layers.")]
    public void Pulse_to_flow()
    {
        var latch = TheLatch.Holds<int>();
        Signal.From(ToFlow_get_flow())
            .SetArtery(latch)
            .Pulse(41);
        Assert.Equal(42, latch.Q);
    }

    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<int> ToFlow_get_flow()
    {
        var subflow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Trace<int>(a => input + a)
            select input;
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Prime(() => 1)
            from __ in Pulse.ToFlow(subflow, input)    // <=
            select input;
        // Pulse 41 => results in 42.
        return flow;
    }

    [Fact]
    [DocContent("`ToFlow` can also iterate through collections:")]
    [DocExample(typeof(Circulation), nameof(ToFlow_get_collection_flow))]
    [DocContent("This version of `ToFlow` is the declarative way to write what would otherwise be a `loop`, `foreach`, `for`, etcetera.")]
    public void Pulse_to_flow_collection()
    {
        var latch = TheLatch.Holds<string>();
        Signal.From(ToFlow_get_collection_flow())
            .SetArtery(latch)
            .Pulse([1, 2, 3]);
        Assert.Equal("Sum = 6", latch.Q);
    }

    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<List<int>> ToFlow_get_collection_flow()
    {
        var subflow =
            from input in Pulse.Start<int>()
            from result in Pulse.Manipulate<int>(a => a + input)
            select input;
        var flow =
            from input in Pulse.Start<List<int>>()
            from _1 in Pulse.Prime(() => 0)
            from _2 in Pulse.ToFlow(subflow, input)
            from _3 in Pulse.Trace<int>(a => $"Sum = {a}")
            select input;
        // Pulse [1, 2, 3] => results in "Sum = 6".
        return flow;
    }

    [Fact]
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

    [Fact]
    [DocHeader("Query Syntax vs Method Syntax")]
    [DocContent(
@"> Maybe now is the time to talk about Kevin.  

Another feature of LINQ is the two syntactically different but computationally equal styles of expression.  
In general the query syntax is more declarative (*what* you want to do),
while the method syntax can be more practical (*how* it actually executes).  

QuickPulse offers two similar dialects. The examples above are written in what could be called QuickPulse **query syntax**.  
Here are the same examples rewritten using **method syntax**:")]
    [DocExample(typeof(Circulation), nameof(Then_get_flow_method))]
    [DocExample(typeof(Circulation), nameof(ToFlow_get_flow_method))]
    [DocExample(typeof(Circulation), nameof(ToFlow_get_collection_flow_method))]
    public void ToFlow_method_syntax()
    {
        var holden = TheString.Catcher();
        Signal.From(Then_get_flow_method())
            .SetArtery(holden)
            .Pulse(42);
        Assert.Equal("... 42", holden.Whispers());

        var latch1 = TheLatch.Holds<int>();
        Signal.From(ToFlow_get_flow_method())
            .SetArtery(latch1)
            .Pulse(41);
        Assert.Equal(42, latch1.Q);

        var latch2 = TheLatch.Holds<string>();
        Signal.From(ToFlow_get_collection_flow_method())
            .SetArtery(latch2)
            .Pulse([1, 2, 3]);
        Assert.Equal("Sum = 6", latch2.Q);
    }

    [CodeSnippet]
    [CodeRemove("return ")]
    private static Flow<int> Then_get_flow_method()
    {
        var dot = Pulse.Trace(".");
        var space = Pulse.Trace(" ");
        return Pulse.Start<int>(a =>
            dot.Then(dot).Then(dot).Then(space).Then(Pulse.Trace(a)));
    }

    [CodeSnippet]
    [CodeRemove("return ")]
    private static Flow<int> ToFlow_get_flow_method()
    {
        return Pulse.Start<int>(a =>
            Pulse.Prime(() => 1)
                .Then(Pulse.ToFlow(b => Pulse.Trace<int>(c => b + c), a)));
    }

    [CodeSnippet]
    [CodeRemove("return ")]
    private static Flow<List<int>> ToFlow_get_collection_flow_method()
    {
        return Pulse.Start<List<int>>(numbers =>
            Pulse.Prime(() => 0)
                .Then(Pulse.ToFlow(a => Pulse.Manipulate<int>(b => a + b).Dissipate(), numbers))
                .Then(Pulse.Trace<int>(a => $"Sum = {a}")));
    }

    [Fact]
    [DocContent(
@"Ultimately, the choice between query syntax and method syntax comes down to readability and personal preference.
Query syntax often provides a more declarative, linear flow that clearly expresses the sequence of operations,
while method syntax can offer a more functional, compositional style that some developers find more natural.  

**Note:** The .`Dissipate()` extension method runs the targeted flow and discards its output, returning a `Flow<Flow>`.  
It's often used in method syntax to glue flows together seamlessly.")]
    public void Dissipate()
    {
        var flow = Pulse.Start<int>(a => Pulse.NoOp());
        Assert.IsType<Flow<int>>(flow);
        Assert.IsType<Flow<Flow>>(flow.Dissipate());
    }
}