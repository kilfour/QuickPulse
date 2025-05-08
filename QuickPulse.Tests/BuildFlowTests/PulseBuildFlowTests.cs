using QuickPulse.Bolts;
using QuickPulse.Tests._Tools;

namespace QuickPulse.Tests.BuildFlowTests;


[Doc(Order = Chapters.BuildFlow, Caption = "Building a Flow", Content =
@"In order to explain how QuickPulse works, not in the least to myself, let's walk through 
building up a flow, for a real world use case.
")]
public class PulseBuildFlowTests
{
    [Doc(Order = Chapters.BuildFlow + "-1", Content =
@"The minimal definition of a flow :
```csharp
from _ in Pulse.Start<DocAttribute>()
select Pulse.Stop;
```
The type generic in `Pulse.Start` defines the **input type** to the flow.  
This means you'll call it like this:
```csharp
flow.Run(new DocAttribute());
```
")]
    [Fact]
    public void Minimal_definition_start()
    {
        var flow =
            from _ in Pulse.Start<DocAttribute>()
            select Pulse.Stop;
        //Assert.IsType<Unit>(flow.Run(new DocAttribute()));
    }

    [Doc(Order = Chapters.BuildFlow + "-2", Content =
@"`Pulse.Stop` is just semantic sugar, a readable way to express that the return value is irrelevant.  
While you could select anything (including real return values),
using Pulse.Stop makes your intent clear to both the compiler and future readers.")]
    [Fact]
    public void Minimal_definition_stop()
    {
        var flow =
            from _ in Pulse.Start<DocAttribute>()
            select Pulse.Stop;
        Assert.IsType<Flow<Unit>>(flow);
    }

    [Doc(Order = Chapters.BuildFlow + "-4", Content =
@"**Adding a Shape:**


")]
    [Fact]
    public void Adding_a_Shape()
    {
        var flow =
            from doc in Pulse.Start<DocAttribute>()
            let caption = doc.Caption
            select Pulse.Stop;
        Assert.IsType<Flow<Unit>>(flow);
    }

    [Doc(Order = Chapters.BuildFlow + "-3", Content =
@"**Adding a Trace:**
You might have guessed were building a document generator thingy, 

")]
    [Fact]
    public void Filtering()
    {
        var flow =
            from doc in Pulse.Start<DocAttribute>()
            from _ in Pulse.Trace(() => { })
            select Pulse.Stop;
        Assert.IsType<Flow<Unit>>(flow);
    }

    // [Fact]
    // public void Two_headings()
    // {
    //     var collector = new List<string>();
    //     var flow = GetFlow(collector);

    //     flow.Run(new DocAttribute(order: "1", caption: "Heading", content: ""));
    //     flow.Run(new DocAttribute(order: "2-1", caption: "Another", content: ""));

    //     Assert.Equal(2, collector.Count);
    //     Assert.Equal("# Heading", collector[0]);
    //     Assert.Equal("## Another", collector[1]);
    // }

    // [Fact]
    // public void Check_lines()
    // {
    //     var collector = new List<string>();
    //     var flow = GetFlow(collector);

    //     flow.Run(new DocAttribute(order: "1", caption: "Heading", content: ""));
    //     flow.Run(new DocAttribute(order: "2", caption: "Another", content: ""));

    //     Assert.Equal(3, collector.Count);
    //     Assert.Equal("# Heading", collector[0]);
    //     Assert.Equal("---", collector[1]);
    //     Assert.Equal("# Another", collector[2]);
    // }

    // public class Collector : IPulser
    // {
    //     public readonly List<string> exhibit = [];
    //     public void Monitor(params object[] data)
    //     {
    //         foreach (var item in data)
    //         {
    //             exhibit.Add((string)item);
    //         }
    //     }
    // }

}