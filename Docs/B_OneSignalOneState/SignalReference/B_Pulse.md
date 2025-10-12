# Pulse
## Pulsing One Value
`Signal.Pulse(...)` is the main way a flow can be instructed to do useful work.  
```csharp
var collector = TheCollector.Exhibits<int>();
Signal.From<int>(a => Pulse.Trace(a))
    .SetArtery(collector)
    .Pulse(42);
Assert.Single(collector.TheExhibit);
Assert.Equal(42, collector.TheExhibit[0]);
```
As the `Assert`'s demonstrate, this sends the int `42` into the flow.  
## Pulsing Many Values
For ease of use, when dealing with `IEnumerable` return values from various sources, an overload exists: `Signal.Pulse(IEnumerable<T> inputs)`.   
```csharp
var collector = TheCollector.Exhibits<int>();
Signal.From<int>(a => Pulse.Trace(a))
    .SetArtery(collector)
    .Pulse([42, 43, 44]);
Assert.Equal(3, collector.TheExhibit.Count);
Assert.Equal(42, collector.TheExhibit[0]);
Assert.Equal(43, collector.TheExhibit[1]);
Assert.Equal(44, collector.TheExhibit[2]);
```
Same behaviour as the single-pulse example.  
## Pulsing Nothing
Lastly, in some rare circumstances, a flow does not take any input. In `QuickPulse` *nothing* is represented by a `Unit` type.  
So in order to advance a flow of type `Flow<Unit>` you can use the `Signal.Pulse()` overload.  
```csharp
var flow =
    from input in Pulse.Start<Unit>()
    from _1 in Pulse.Prime(() => 42)
    from _2 in Pulse.Trace<int>(a => a)
    from _3 in Pulse.Manipulate<int>(a => a + 1)
    select input;
var collector = TheCollector.Exhibits<int>();
Signal.From(flow)
    .SetArtery(collector)
    .Pulse().Pulse().Pulse();
Assert.Equal(3, collector.TheExhibit.Count);
Assert.Equal(42, collector.TheExhibit[0]);
Assert.Equal(43, collector.TheExhibit[1]);
Assert.Equal(44, collector.TheExhibit[2]);
```
