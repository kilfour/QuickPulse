# Trace

**`Pulse.Trace(...)`** emits trace data unconditionally to the current artery.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.Trace(anInt) // <=
select anInt;
```


**`Pulse.TraceIf(...)`** emits trace data conditionally, based on a boolean flag.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from _ in Pulse.TraceIf(anInt != 42, () => anInt) // <=
select anInt;
```

