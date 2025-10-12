# Make It Flow
> Building larger behaviours from tiny flows.  

QuickPulse is about *composing* small, predictable `Flow<T>` building blocks. 
This chapter shows how to wire those flows together.  
## Then
`Then` runs `flow`, discards its value, and continues with `next` in the **same** state.
It's the flow-level equivalent of do this, *then* do that.  
```csharp
var dot = Pulse.Trace(".");
var space = Pulse.Trace(" ");
var flow =
    from input in Pulse.Start<int>()
    from _1 in dot.Then(dot).Then(dot).Then(space) // <=
    from _2 in Pulse.Trace(input)
    select input;
// Pulse 42 => results in '... 42'.
```
## ToFlow
Given this *sub* flow:  
```csharp
private static Flow<int> SubFlow()
{
    return
        from input in Pulse.Start<int>()
        from _ in Pulse.Trace(input + 1)
        select input;
}
```
`Pulse.ToFlow(...)` Executes a subflow over a value.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.ToFlow(SubFlow(), input)    // <=
    select input;
var latch = TheLatch.Holds<int>();
Signal.From(flow).SetArtery(latch).Pulse(41);
Assert.Equal(42, latch.Q);
```
An overload exist that allows for executing a subflow over a collection of values.  
```csharp
var flow =
    from input in Pulse.Start<List<int>>()
    from _ in Pulse.ToFlow(SubFlow(), input)    // <=
    select input;
var collector = TheCollector.Exhibits<int>();
var signal = Signal.From(flow).SetArtery(collector);
signal.Pulse([41, 41]);
Assert.Equal([42, 42], collector.TheExhibit);
```
Furthermore both the above methods can be used with a *Flow Factory Method*.  
Single value:  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.ToFlow(a => Pulse.Trace(a + 1), input)    // <=
    select input;
var latch = TheLatch.Holds<int>();
var signal = Signal.From(flow).SetArtery(latch);
signal.Pulse(41);
Assert.Equal(42, latch.Q);
```
Multiple values:  
```csharp
var flow =
    from input in Pulse.Start<List<int>>()
    from _ in Pulse.ToFlow(a => Pulse.Trace(a + 1), input)    // <=
    select input;
var collector = TheCollector.Exhibits<int>();
var signal = Signal.From(flow).SetArtery(collector);
signal.Pulse([41, 41]);
Assert.Equal([42, 42], collector.TheExhibit);
```
