using QuickPulse.Arteries;
using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.E_CapillariesAndArterioles.Combinators;


[DocFileHeader("The `Pulse.{SomeMethod}If()` Variants")]
[DocContent(
@"In a similar vein to the state aware utility overloads,
most `Pulse` methods have an `If` variant that allows for conditional execution.  

")]
public class TheSomethingIfVariants
{
    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<int> TraceIf_flow()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.TraceIf(input % 2 == 0, () => "even")
            select input;
        // Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
        return flow;
    }

    [Fact]
    [DocContent("**Examples:**  \n")]
    [DocContent("*Conditional tracing:*")]
    [DocExample(typeof(TheSomethingIfVariants), nameof(TraceIf_flow))]
    public void TraceIf()
    {
        var collector = Collect.ValuesOf<string>();
        Signal.From(TraceIf_flow())
            .SetArtery(collector)
            .Pulse([1, 2, 3, 4, 5]);
        Assert.Equal(["even", "even"], collector.Values);
    }

    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<int> ToFlowIf_flow()
    {
        var even = Pulse.Start<int>(_ => Pulse.Trace("even"));
        var three = Pulse.Start<int>(_ => Pulse.Trace("three"));
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.ToFlowIf(input % 2 == 0, even, () => input)
            from __ in Pulse.ToFlowIf(input == 3, three, () => input)
            select input;
        // Pulse [1, 2, 3, 4, 5] => results in ["even", "three", "even"].
        return flow;
    }

    [Fact]
    [DocContent("*Branching a flow:*")]
    [DocExample(typeof(TheSomethingIfVariants), nameof(ToFlowIf_flow))]
    public void ToFlowIf()
    {
        var collector = Collect.ValuesOf<string>();
        Signal.From(ToFlowIf_flow())
            .SetArtery(collector)
            .Pulse([1, 2, 3, 4, 5]);
        Assert.Equal(["even", "three", "even"], collector.Values);
    }

    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<int> ManipulateIf_flow()
    {
        var even = Pulse.Start<int>(_ => Pulse.Trace("even"));
        var three = Pulse.Start<int>(_ => Pulse.Trace("three"));
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.Prime(() => 0)
            from __ in Pulse.ManipulateIf<int>(input % 2 == 0, a => a + 1)
            from ___ in Pulse.Trace<int>(a => $"{input}: {a}")
            select input;
        // Pulse [1, 2, 3, 4, 5] => results in ["1: 0", "2: 1", "3: 1", "4: 2", "5: 2"].
        return flow;
    }

    [Fact]
    [DocContent("*Counting even numbers using `ManipulateIf()`:*")]
    [DocExample(typeof(TheSomethingIfVariants), nameof(ManipulateIf_flow))]
    public void ManipulateIf()
    {
        var collector = Collect.ValuesOf<string>();
        Signal.From(ManipulateIf_flow())
            .SetArtery(collector)
            .Pulse([1, 2, 3, 4, 5]);
        Assert.Equal(["1: 0", "2: 1", "3: 1", "4: 2", "5: 2"], collector.Values);
    }

}