## QuickPulse
Do you know how you sometimes leave your house—maybe to get some cigarettes—and start thinking about something?
Your brain takes over.
You walk straight past the shop, and the legs just keep going.
An hour later, you look up, and you're in the next village wondering how you got there.

No? Just me?

Well, okay.

It happens in code too, ... quite a lot.
This library is the result of one of those walks through a dark forest.
And yes, it did *literally* involve Trees.

---
## Building a Flow
To explain how QuickPulse works (not least to myself), let's build up a flow step by step.

### The Minimal Flow

```csharp
from anInt in Pulse.Start<int>()
select anInt;
```

The type generic in `Pulse.Start<T>` defines the **input type** to the flow.
**Note:** It is required to select the result of `Pulse.Start(...)` at the end of the LINQ chain for the flow to be considered well-formed.

---
### Doing Something with the Input
Let's trace the values as they pass through:

```csharp
from anInt in Pulse.Start<int>()
from trace in Pulse.Trace(anInt)
select anInt;
```


---
### Executing a Flow
To execute a flow, we need a `Signal<T>`, which is created via:

```csharp
Signal.From<T>(Flow<T> flow)
```

Example:

```csharp
var flow =
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;

var signal = Signal.From(flow);
```


---
### Sending Values Through the Flow
Once you have a signal, you can push values into the flow by calling:

```csharp
Signal.Pulse(params T[] input)
```

Example:

```csharp
var flow =
    from anInt in Pulse.Start<int>()
    from trace in Pulse.Trace(anInt)
    select anInt;

var signal = Signal.From(flow);
signal.Pulse(42);
```

This sends the value `42` into the flow.


---
### Capturing the Trace
**Capturing the trace**

To observe what flows through, we can add an `IArtery`.
There are a few ways to do this—here’s one using `SetArtery` directly on the signal.

```csharp
[Fact]
public void Adding_an_artery()
{
    var flow =
        from anInt in Pulse.Start<int>()
        from trace in Pulse.Trace(anInt)
        select anInt;

    var collector = new TheCollector<int>();

    Signal.From(flow)
        .SetArtery(collector)
        .Pulse(42, 43, 44);

    Assert.Equal(3, collector.TheExhibit.Count);
    Assert.Equal(42, collector.TheExhibit[0]);
    Assert.Equal(43, collector.TheExhibit[1]);
    Assert.Equal(44, collector.TheExhibit[2]);
}
```



