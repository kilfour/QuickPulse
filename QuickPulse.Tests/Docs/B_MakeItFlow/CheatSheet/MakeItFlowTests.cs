using QuickPulse.Explains;
using QuickPulse.Explains.Deprecated;

namespace QuickPulse.Tests.Docs.B_MakeItFlow.CheatSheet;

[DocFile]
[DocContent(
@"**Number One's Cheat Sheet:**

| Combinator                                    | Role / Purpose                                                                |
| --------------------------------------------- | ----------------------------------------------------------------------------- |
| [**Start<T>()**](./MakeItFlowStart.md)        | Starts a new flow. Defines the input type.                                    |
| [**Trace(...)**](./MakeItFlowTrace.md)        | Emits trace data unconditionally to the current artery.                       |
| [**TraceIf(...)**](./MakeItFlowTrace.md)      | Emits trace data conditionally, based on a boolean flag.                      |
| [**FirstOf(...)**](./MakeItFlowFirstOf.md)    | Executes the first flow where its condition is `true`, skips the rest.        |
| [**Effect(...)**](./MakeItFlowEffect.md)      | Performs a side-effect (logging, mutation, etc.) without yielding a value.    |
| [**EffectIf(...)**](./MakeItFlowEffect.md)    | Performs a side-effect conditionally.                                         |
| [**Gather<T>(...)**](./MakeItFlowGather.md)   | Captures a mutable box into flow memory (first write wins).                   |
| [**Scoped<T>(...)**](./MakeItFlowScoped.md)   | Temporarily mutates gathered state during a subflow, then restores it.        |
| [**ToFlow(...)**](./MakeItFlowToFlow.md)      | Invokes a subflow over a value or collection.                                 |
| [**ToFlowIf(...)**](./MakeItFlowToFlow.md)    | Invokes a subflow conditionally, using a supplier for the input.              |
| [**When(...)**](./MakeItFlowWhen.md)          | Executes the given flow only if the condition is true, without input.         |
| [**NoOp()**](./MakeItFlowNoOp.md)             | Applies a do-nothing operation (for conditional branches or comments).        |
")]
public class MakeItFlowTests
{
    // ------------
    // PLACEHOLDER  
    // ------------  
}