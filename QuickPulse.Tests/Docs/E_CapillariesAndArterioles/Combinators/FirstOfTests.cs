using QuickPulse.Arteries;
using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.E_CapillariesAndArterioles.Combinators;

[DocFileHeader("FirstOf")]
[DocContent(
@"Pulse.FirstOf(...) lets you chain multiple conditional flows and automatically
runs the first one whose condition evaluates to true.
It's like a compact, declarative if / else if / else ladder for flows."
)]
public class FirstOfTests
{
    [CodeSnippet]
    [CodeRemove("return flow;")]
    private static Flow<int> FirstOf_flow()
    {
        var flow =
            from input in Pulse.Start<int>()
            from _ in Pulse.FirstOf(
                (() => input % 2 == 0, () => Pulse.Trace("even")),
                (() => input == 3, () => Pulse.Trace("three")))
            select input;
        // Pulse [1, 2, 3, 4, 5] => results in ["even", "three", "even"].
        return flow;
    }

    [Fact]
    [DocExample(typeof(FirstOfTests), nameof(FirstOf_flow))]
    public void FirstOf()
    {
        var collector = TheCollector.Exhibits<string>();
        Signal.From(FirstOf_flow())
            .SetArtery(collector)
            .Pulse([1, 2, 3, 4, 5]);
        Assert.Equal(["even", "three", "even"], collector.TheExhibit);
    }
}