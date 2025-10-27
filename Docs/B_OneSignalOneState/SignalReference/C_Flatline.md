# Flatline
`Signal.FlatLine(...)` is a terminal operation that runs a final flow once the main signal has completed pulsing.
It's useful for summarizing, tracing, or cleaning up after a sequence of pulses.  
```csharp
    Signal.From(
            from _ in Pulse.Start<Flow>()
            from __ in Pulse.Prime(() => 0)
            from ___ in Pulse.Manipulate<int>(a => a + 1)
            select Flow.Continue
        )
        .SetArtery(TheLatch.Holds<int>())
        .Pulse().Pulse().Pulse()
        .FlatLine(Pulse.Trace<int>(a => a))
        .GetArtery<Latch<int>>()
        .Q; // <= returns 3
```
