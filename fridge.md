# Combinator Doc
| Combinator         | Role/Purpose                                                               |
| ------------------ | -------------------------------------------------------------------------- |
| **Start<T>()**     | Entry point for a flow. Required to begin any LINQ chain.                  |
| **Using(...)**     | Assigns an `IArtery` to the flow context — enables tracing.                |
| **Trace(...)**     | Emits trace data unconditionally to the current artery.                    |
| **TraceIf(...)**   | Emits trace data conditionally, based on a boolean flag.                   |
| **Effect(...)**    | Executes a side-effect (logging, mutation, etc.) without yielding a value. |
| **EffectIf(...)**  | Same as above, but conditional.                                            |
| **Gather<T>(...)** | Binds a mutable box into flow memory (first write wins).                   |
| **ToFlow(...)**    | Executes a flow over a value or collection — useful for subflows.          |
| **ToFlowIf(...)**  | Executes a subflow conditionally, using a supplier for the input.          |
| **NoOp()**         | A do-nothing flow (useful for conditional branches).                       |

--- slide ---
# Combinator Doc 2
### Pulse.Trace(...)

Emits values to the current artery.

**Use when:** You want to observe values flowing through the pipeline.

```csharp
from _ in Pulse.Trace("Hello", someValue)
```
--- slide ---