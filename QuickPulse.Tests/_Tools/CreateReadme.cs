using QuickPulse.Explains;

namespace QuickPulse.Tests._Tools;

public class CreateReadme
{
    [Fact]
    [Doc(Order = "0", Caption = "QuickPulse Documentation", Content =
@"
- [Introduction](Introduction.md)
- [Building a Flow](BuildingAFlow.md)
- [How to Pulse](HowToPulse.md)
- [Pulsing A Flow](PulsingAFlow.md)
- [Flow Extensions](FlowExtensions.md)
- [Arteries Included](ArteriesIncluded.md)
- [Examples](Examples.md)
- [Why QuickPulse Exists](WhyQuickPulseExists.md)
- [Addendum: No Where](NoWhere.md)
")]
    public void FromDocAttributes()
    {
        new Document().ToFiles([
            new ("./Docs/ToC.md", ["QuickPulse.Tests._Tools"]),

            new ("./Docs/Introduction.md", ["QuickPulse.Tests.Docs.Introduction"]),
            new ("./Docs/BuildingAFlow.md", ["QuickPulse.Tests.Docs.BuildingAFlow"]),
            new ("./Docs/HowToPulse.md", ["QuickPulse.Tests.Docs.HowToPulse"]),
            new ("./Docs/PulsingAFlow.md", ["QuickPulse.Tests.Docs.PulsingAFlow"]),
            new ("./Docs/FlowExtensions.md", ["QuickPulse.Tests.Docs.FlowExtensions"]),
            new ("./Docs/ArteriesIncluded.md", ["QuickPulse.Tests.Docs.ArteriesIncluded"]),
            new ("./Docs/Examples.md", ["QuickPulse.Tests.Docs.Examples"]),
            new ("./Docs/WhyQuickPulseExists.md", ["QuickPulse.Tests.Docs.WhyQuickPulseExists"]),
            new ("./Docs/NoWhere.md", ["QuickPulse.Tests.Docs.NoWhere"]),

        ], typeof(CreateReadme).Assembly);
    }
}