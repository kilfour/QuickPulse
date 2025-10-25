using QuickPulse.Arteries;
using QuickPulse.Explains;
using QuickPulse.Explains.Formatters;
using QuickPulse.Tests.Docs.E_BendingTheRiver.Combinators;

namespace QuickPulse.Tests.Docs.E_OnCapillariesAndArterioles;

[DocFile]
[DocFileHeader("Capillaries and Arterioles")]
[DocContent(
@"> A.k.a. Pulse Regulation

This chapter explores and explains the various ways you can control the execution of a `Flow`.

All flow examples below will be executed using the following `Signal`:")]
[DocExample(typeof(CapillariesAndArterioles), nameof(GetResult))]
[DocContent("In addition, unless explicitly specified the result of executing the example flow will always be: ")]
[DocExample(typeof(CapillariesAndArterioles), nameof(Default_result_values), "bash")]
// [DocInclude(typeof(UsingConditionalTernaryOperator))]
// [DocInclude(typeof(WhenTests))]
// [DocInclude(typeof(TraceIfTests))]
// [DocInclude(typeof(ToFlowIfTests))]
public class CapillariesAndArterioles
{
    [CodeSnippet]
    [CodeRemove("return ")]
    public static IEnumerable<string> GetResult(Flow<int> flow)
    {
        return Signal.From(flow)
            .SetArtery(TheCollector.Exhibits<string>())
            .Pulse([1, 2, 3, 4, 5])
            .GetArtery<Collector<string>>()
            .TheExhibit;
    }

    [CodeSnippet]
    [CodeFormat(typeof(StringArrayToString))]
    public static string[] Default_result_values()
    {
        return ["even", "even"];
    }

    [Fact]
    public void Foo()
    {
        Explain.This<CapillariesAndArterioles>("temp.md");
    }
}