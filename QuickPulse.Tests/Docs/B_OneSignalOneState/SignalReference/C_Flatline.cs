using QuickPulse.Arteries;
using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.B_OneSignalOneState.SignalReference;

[DocContent(
@"`Signal.FlatLine(...)` is a terminal operation that runs a final flow once the main signal has completed pulsing.
It's useful for summarizing, tracing, or cleaning up after a sequence of pulses.  

The following example does use some features fully explained in the chapter **'Memory And Manipulation'**.")]
public class C_Flatline
{
    [Fact]
    [DocExample(typeof(C_Flatline), nameof(FlatLine_runs_on_terminal_state_example))]
    public void FlatLine_runs_on_terminal_state()
    {
        var latch = TheLatch.Holds<int>();
        FlatLine_runs_on_terminal_state_example(latch);
        Assert.Equal(3, latch.Q);
    }

    [CodeSnippet]
    [CodeRemove(".SetArtery(latch)")]
    [CodeReplace(");", ";")]
    private static void FlatLine_runs_on_terminal_state_example(Latch<int> latch)
    {
        var flow =
            from _ in Pulse.Start<Flow>()
            from __ in Pulse.Prime(() => 0)
            from ___ in Pulse.Manipulate<int>(a => a + 1)
            select Flow.Continue;
        Signal.From(flow)
            .SetArtery(latch)
            .Pulse().Pulse().Pulse()
            .FlatLine(Pulse.Trace<int>(a => a));
        // Results in => 3
    }
}