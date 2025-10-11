using QuickPulse.Explains;
using QuickPulse.Arteries;

namespace QuickPulse.Tests.Docs.D_FlowExtensions;

[DocFile]
[DocContent(
@"Not a big fan of extensions on LINQ enabled combinators, but there are a couple which are just to useful to pass up on.")]
public class FlowExtensionsTests
{
    [Fact]
    [DocHeader("Then")]
    [DocContent(
@"**`.Then(...)`** is just syntactic sugar for `.SelectMany(...)`.

Suppose we have: 
```csharp
var dot = Pulse.Trace(""."");
var space = Pulse.Trace("" "");
```
We can compose this like so:
```csharp
var threeDotsAndSpace =
    from d1 in dot
    from d2 in dot
    from d3 in dot
    from s in space
    select Unit.Instance;
```
Most of you would probably prefer: 
```csharp
var threeDotsAndSpace = dot.SelectMany(_ => dot).SelectMany(_ => dot).SelectMany(_ => space);
```
Now with `.Then(...)` you can do:
```csharp
var threeDotsAndSpace = dot.Then(dot).Then(dot).Then(space);
```
")]
    public void Example()
    {
        var dot = Pulse.Trace(".");
        var space = Pulse.Trace(" ");
        Assert.Equal("... 42",
            Signal.From(
                    from input in Pulse.Start<int>()
                    from _ in dot.Then(dot).Then(dot).Then(space)
                    from rest in Pulse.Trace($"{input}")
                    select input)
                .SetArtery(TheString.Catcher())
                .Pulse(42)
                .GetArtery<Holden>()
                .Whispers());
    }
}