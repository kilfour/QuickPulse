# FirstOf

**`Pulse.FirstOf(...)`** runs the first flow in a sequence of (condition, flow) pairs where the condition evaluates to true.

**Example:**
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.FirstOf(
        (() => input == 42, () => Pulse.Trace("answer")),
        (() => input == 666, () => Pulse.Trace("beëlzebub")),
        (() => input == 42 || input == 666, () => Pulse.Trace("never")))
    select input;
var collector = new TheCollector<string>();
Signal.From(flow).SetArtery(collector).Pulse(1, 42, 666, 7);
Assert.Equal(["answer", "beëlzebub"], collector.TheExhibit);
```

