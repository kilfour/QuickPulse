using QuickPulse.Explains;

namespace QuickPulse.Tests._Tools;

public class CreateReadme
{
    [Fact]
    [Doc(Order = "0", Caption = "QuickPulse Documentation", Content =
@"
- [Building a Flow](BuildingAFlow.md)
- [How to Pulse](HowToPulse.md)
- [Pulsing A Flow](PulsingAFlow.md)
- [Flow Extensions](FlowExtensions.md)
- [Arteries Included](ArteriesIncluded.md)
- [Examples](Examples.md)
- [Addendum: No Where](NoWhere.md)
")]
    public void FromDocAttributes()
    {
        new Document().ToFiles([
            new ("README.md", ["QuickPulse.Tests.Docs.Introduction"]), // Check

            new ("./Docs/ToC.md", ["QuickPulse.Tests._Tools"]),

            new ("./Docs/BuildingAFlow.md", ["QuickPulse.Tests.Docs.BuildingAFlow"]), // Check
            new ("./Docs/HowToPulse.md", ["QuickPulse.Tests.Docs.HowToPulse"]),
            new ("./Docs/PulsingAFlow.md", ["QuickPulse.Tests.Docs.PulsingAFlow"]),
            new ("./Docs/FlowExtensions.md", ["QuickPulse.Tests.Docs.FlowExtensions"]),
            new ("./Docs/ArteriesIncluded.md", ["QuickPulse.Tests.Docs.ArteriesIncluded"]), // Check
            new ("./Docs/Examples.md", ["QuickPulse.Tests.Docs.Examples"]),
            // new ("./Docs/WhyQuickPulseExists.md", ["QuickPulse.Tests.Docs.WhyQuickPulseExists"]),
            new ("./Docs/NoWhere.md", ["QuickPulse.Tests.Docs.NoWhere"]), // Check

        ], typeof(CreateReadme).Assembly);
    }
}