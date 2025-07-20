using QuickPulse.Explains;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Docs.Introduction;


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
    public void Example() =>
        Assert.Equal("A deep dark forest, a looking glass and a trail of dead generators.",
            Signal.From(
                    from input in Pulse.Start<string>()
                    from isFirst in Pulse.Gather(true)
                    let capitalized = char.ToUpper(input[0]) + input[1..]
                    let evenLength = input.Length % 2 == 0
                    from _1 in Pulse.TraceIf(isFirst.Value, capitalized)
                    from _2 in Pulse.TraceIf(!isFirst.Value, $" {input}")
                    from _3 in Pulse.TraceIf(evenLength, ", a looking glass")
                    from _ in Pulse.Effect(() => isFirst.Value = false)
                    select input)
                .SetArtery(TheString.Catcher())
                .Pulse("a deep dark forest")
                .Pulse("and a trail of dead generators.")
                .GetArtery<Holden>()
                .Whispers());
}