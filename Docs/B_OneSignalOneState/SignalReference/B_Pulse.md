# Pulse
**`Signal.Pulse(...)`** is the main way a flow can be instructed to do useful work.  
```csharp
var collector = TheCollector.Exhibits<int>();
Signal.From<int>(a => Pulse.Trace(a))
    .SetArtery(collector)
    .Pulse(42);
Assert.Single(collector.TheExhibit);
Assert.Equal(42, collector.TheExhibit[0]);
```
As the `Assert`'s demonstrate, this sends the int `42` into the flow.  
For ease of use, when dealing with `IEnumerable` return values from various sources, an overload exists: `Pulse(IEnumerable<T> inputs)`.   
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
This behaves exactly like the previous example.  
