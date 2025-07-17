using System.Text;
using QuickExplainIt;
using QuickExplainIt.Text;
using QuickPulse.Arteries;
using QuickPulse.Tests._Tools;

namespace QuickPulse.Tests.Docs.IntroTests;


[Doc(Order = Chapters.Introduction, Caption = "QuickPulse", Content =
@"> LINQ with a heartbeat.

Do you know how you sometimes leave your house, maybe to get some cigarettes, and start thinking about something?
Your brain takes over.
You walk straight past the shop, and the legs just keep going.
An hour later, you look up, and you're in the next village wondering how you got there.

No? Just me?

Well, okay.

It happens in code too, ... quite a lot.
This library is the result of one of those walks through a dark forest.
And yes, it did *literally* involve Trees.")]
public class PulseIntroTests
{
    [Fact]
    [Doc(Order = Chapters.Introduction + "-1", Caption = "QuickPulse", Content =
@"```
Assert.Equal(""A deep dark forest, a looking glass and a trail of dead generators."",
    Signal.From(
            from input in Pulse.Start<string>()
            from isFirst in Pulse.Gather(true)
            from first in Pulse.TraceIf(isFirst.Value, char.ToUpper(input[0]) + input[1..])
            from rest in Pulse.TraceIf(!isFirst.Value, $"" {input}"")
            from off in Pulse.Effect(() => isFirst.Value = false)
            from even in Pulse.TraceIf(input.Length % 2 == 0, $"", a looking glass"")
            select input)
        .SetArtery(TheString.Catcher())
        .Pulse(""a deep dark forest"")
        .Pulse(""and a trail of dead generators."")
        .GetArtery<Holden>()
        .Whispers());```")]
    public void Example() =>
        Assert.Equal("A deep dark forest, a looking glass and a trail of dead generators.",
            Signal.From(
                    from input in Pulse.Start<string>()
                    from isFirst in Pulse.Gather(true)
                    from first in Pulse.TraceIf(isFirst.Value, char.ToUpper(input[0]) + input[1..])
                    from rest in Pulse.TraceIf(!isFirst.Value, $" {input}")
                    from off in Pulse.Effect(() => isFirst.Value = false)
                    from even in Pulse.TraceIf(input.Length % 2 == 0, $", a looking glass")
                    select input)
                .SetArtery(TheString.Catcher())
                .Pulse("a deep dark forest")
                .Pulse("and a trail of dead generators.")
                .GetArtery<Holden>()
                .Whispers());
}