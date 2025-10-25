using QuickPulse.Arteries;
using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs.B_OneSignalOneState.SignalReference;

[DocFile]
[DocContent(
@"`Signal.From(...)` is a simple factory method used to get hold of a `Signal<T>` instance
that wraps the passed in `Flow<T>`.")]
public class C_Flatline
{
    [Fact]
    public void FlatLine_runs_on_terminal_state()
    {
        var flow =
            from input in Pulse.Start<Unit>()
            from _1 in Pulse.Prime(() => 0)
            from _2 in Pulse.Manipulate<int>(a => a + 1)
            select input;

        Assert.True(
            Signal.From(Pulse.Prime(() => false).Dissipate())
            .SetArtery(TheLatch.Holds<bool>())
            .FlatLine(Pulse.Trace(true))
            .GetArtery<Latch<bool>>().Q);
    }
}