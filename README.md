# QuickPulse

> **LINQ with a heartbeat.**
>
> A tiny library for building stateful, inspectable, composable flows.

## Getting Started

QuickPulse lets you write LINQ expressions that react to values, accumulate state, and emit traces along the way.

```csharp
Signal.From(
        from input in Pulse.Start<string>()
        from isFirst in Pulse.Gather(true)
        let capitalized = char.ToUpper(input[0]) + input[1..]
        let evenLength = input.Length % 2 == 0
        from _1 in Pulse.TraceIf(isFirst.Value, () => capitalized)
        from _2 in Pulse.TraceIf(!isFirst.Value, () => $" {input}")
        from _3 in Pulse.TraceIf(evenLength, () => ", a looking glass")
        from _ in Pulse.Effect(() => isFirst.Value = false)
        select input)
    .SetArtery(TheString.Catcher())
    .Pulse("a deep dark forest")
    .Pulse("and a trail of dead generators.")
    .GetArtery<Holden>()
    .Whispers();

// => "A deep dark forest, a looking glass and a trail of dead generators."
```

## Documentation

QuickPulse is fully documented, with real, executable examples for each combinator and concept.

You can explore it here:

* **[Table of Contents](./Docs/TOC.md)**

Or, ... see [A Quick Pulse](./Docs/AQuickPulse.md) for a hands-on quickstart.

## Highlights

* Composable `Flow<T>` pipelines using LINQ.
* Stateful `Signal<T>` execution.
* Built-in tracing via `IArtery`.
* Conditional logic (`TraceIf`, `EffectIf`, `When`).
* Scoped state and reversible manipulations.
* Integration with file logging and string capture.

## Why QuickPulse Exists?

Do you know how you sometimes leave your house, maybe to get some cigarettes, and start thinking about something?
Your brain takes over.
You walk straight past the shop, and the legs just keep going.
An hour later, you look up, and you're in the next village wondering how you got there.

No? Just me?

Well, okay.

It happens in code too, ... quite a lot.
This library is the result of one of those walks through a dark forest.
And yes, it did *literally* involve Trees.


## Installation

QuickPulse is available on NuGet:

```bash
Install-Package QuickPulse
```

Or via the .NET CLI:

```bash
dotnet add package QuickPulse
```

