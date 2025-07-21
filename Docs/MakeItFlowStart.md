# Start
Every flow definition needs to start with a call to `Pulse.Start()`.
This strongly types the values that the flow can receive.
In addition, the result of the call needs to be used in the select part of the LINQ expression.
This strongly types the flow itself.

**Example:**
```csharp
from anInt in Pulse.Start<int>() // <=
select anInt;
```

