# Roadmap


## Finish the Doc

### Todo 
- `FirstOf`
- `ToFlowAs`
- `TraceToIf`
- `ManipulateIf`
- `ScopedIf`
- `Trace`
- `Circulate`
- `Dissipate`
- `Drain`
- `StopFlowing` + If
- `Valve`
- `Start` factory methods


## Pulse.ToFlowIf

**Purpose**: Conditionally executes a subflow

**Overloads**: 
- `ToFlowIf(bool, Flow<T>, Func<T>)`
- `ToFlowIf(bool, Func<Flow<T>>, Func<T>)`
- `ToFlowIf(Predicate<T>, Flow<T>, Func<T>)`
- etc.

**When to use**: When you need optional processing branches

**Examples**: [Link to conceptual doc section]
**See also**: [Pulse.When], [Pulse.TraceIf]