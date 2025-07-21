using QuickPulse.Explains;

namespace QuickPulse.Tests.Docs;

public class CreateReadme
{
    [Fact]
    [Doc(Order = "0", Caption = "QuickPulse Documentation", Content =
@"
- [A Quick Look](AQuickLook.md)
- [Number One, Make it Flow](NumberOneMakeItFlow.md)
- [One Signal, One State](OneSignalOneState.md)
- [Flow Extensions](FlowExtensions.md)
- [Arteries Included](ArteriesIncluded.md)
- [Examples](Examples.md)
- [Addendum: No Where](NoWhere.md)
")]
    public void FromDocAttributes()
    {
        new Document().ToFiles([
            new ("README.md", ["QuickPulse.Tests.Docs.Introduction"]), // Check

            new ("./Docs/ToC.md", ["QuickPulse.Tests.Docs"]),

            new ("./Docs/AQuickLook.md", ["QuickPulse.Tests.Docs.AQuickLook"]), // Check
            new ("./Docs/NumberOneMakeItFlow.md", ["QuickPulse.Tests.Docs.NumberOneMakeItFlow"]),
            new ("./Docs/OneSignalOneState.md", ["QuickPulse.Tests.Docs.OneSignalOneState"]),
            new ("./Docs/FlowExtensions.md", ["QuickPulse.Tests.Docs.FlowExtensions"]),
            new ("./Docs/ArteriesIncluded.md", ["QuickPulse.Tests.Docs.ArteriesIncluded"]), // Check
            new ("./Docs/Examples.md", ["QuickPulse.Tests.Docs.Examples"]),
            // new ("./Docs/WhyQuickPulseExists.md", ["QuickPulse.Tests.Docs.WhyQuickPulseExists"]),
            new ("./Docs/NoWhere.md", ["QuickPulse.Tests.Docs.NoWhere"]), // Check

        ], typeof(CreateReadme).Assembly);
    }
}