# Gather
**`Pulse.Gather(...)`** Binds a mutable box into flow memory (first write wins).

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1) // <=
select anInt;
```
**`Pulse.Gather<T>()`** used without an argument, serves as a 'getter' of a previously gathered value.

**Example:**
```csharp
from anInt in Pulse.Start<int>()
from box in Pulse.Gather(1)
from val in Pulse.Gather<int>() // <=
from _ in Pulse.Trace(anInt + val.Value)
select anInt;
```
**`Pulse.Gather<T>()`** throws if no value of the requested type is available.
