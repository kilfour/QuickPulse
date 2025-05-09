using QuickPulse.Tests._Tools;

namespace QuickPulse.Tests.Docs.NoWhereTests;

[Doc(Order = Chapters.NoWhere, Caption = "No Where")]
public class PulseNoWhereTests
{
    [Doc(Order = Chapters.NoWhere + "-1", Caption = "Why There Is No `.Where(...)` in QuickPulse LINQ", Content =
@"In standard LINQ-to-objects, the `where` clause is lazily applied and safely filters values *before* any downstream computation happens. This works because `IEnumerable<T>` defers evaluation until iteration.

But **QuickPulse uses monadic LINQ over computation flows** (`Flow<T>`), not sequences. In monadic LINQ, the C# compiler desugars `where` **after** any preceding `let`, `from`, or `select` clauses — and **evaluates them eagerly**.

This means:

```csharp
from x in Flow<T>
where x != null
let y = x.SomeProperty // NRE: still evaluated even if x is null!
```

The `let` runs *before* the `where`, causing runtime exceptions — even though it looks safe.
")]
    public void WhyThereIsNoWhere() { /*placeholder*/}

    [Doc(Order = Chapters.NoWhere + "-2", Caption = "Instead of `where`, use:", Content =
@"
* `Pulse.TraceIf(...)`
* `Pulse.NoOp()`
* Custom `.If(...)` / `.Guard(...)` combinators
* Plain ternary logic inside `SelectMany` chains

Example:

```csharp
from diag in Pulse.Start<DiagnosticInfo>()
from _ in diag.Tags.Contains(""Phase"")
    ? Pulse.Trace(""..."")
    : Pulse.NoOp()
```
")]
    public void InsteadOfWhere() { /*placeholder*/}

    [Doc(Order = Chapters.NoWhere + "-3", Caption = "Why This Matters", Content =
@"
Avoiding `.Where(...)` keeps evaluation order predictable and prevents accidental crashes in:

* Diagnostic flows
* Shrinking logic
* Custom combinators and trace sequences

It's a minor trade-off in exchange for greater composability and correctness.
")]
    public void WhyThisMatters() { /*placeholder*/}
}



