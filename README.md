# <img src='icon.png' width='40' align='top'/> QuickPulse

> **LINQ with a heartbeat.**

A tiny library for building stateful, inspectable, composable flows.

[![Docs](https://img.shields.io/badge/docs-QuickPulse-blue?style=flat-square&logo=readthedocs)](https://github.com/kilfour/QuickPulse/blob/main/Docs/ToC.md)
[![NuGet](https://img.shields.io/nuget/v/QuickPulse.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/QuickPulse)
[![License: MIT](https://img.shields.io/badge/license-MIT-success?style=flat-square)](https://github.com/kilfour/QuickPulse/blob/main/LICENSE)  
## Getting Started
QuickPulse lets you write LINQ expressions that react to values, accumulate state, and emit traces along the way.  
```csharp
 Signal.From(
        from input in Pulse.Start<string>()
        from isFirst in Pulse.Prime(() => true)
        let capitalized = char.ToUpper(input[0]) + input[1..]
        let evenLength = input.Length % 2 == 0
        from _1 in Pulse.TraceIf(isFirst, () => capitalized)
        from _2 in Pulse.TraceIf(!isFirst, () => $" {input}")
        from _3 in Pulse.TraceIf(evenLength, () => ", a looking glass")
        from _ in Pulse.Manipulate<bool>(a => false)
        select input)
    .SetArtery(TheString.Catcher())
    .Pulse("a deep dark forest")
    .Pulse("and a trail of dead generators.")
    .GetArtery<Holden>()
    .Whispers();
// Results in =>
//     "A deep dark forest, a looking glass and a trail of dead generators."
```
## Highlights

* Composable `Flow<T>` pipelines using LINQ.
* Stateful `Signal<T>` execution.
* Built-in tracing via `IArtery`.
* Conditional logic.
* Scoped state and reversible manipulations.
* Integration with file logging and value capture.  
## Installation

QuickPulse is available on NuGet:
```bash
Install-Package QuickPulse
```
Or via the .NET CLI:
```bash
dotnet add package QuickPulse
```  
## Documentation

QuickPulse is fully documented, with real, executable examples for each combinator and concept.

You can explore it here:

* **[Table of Contents](https://github.com/kilfour/QuickPulse/blob/main/Docs/ToC.md)**

Or, ... see [A Quick Pulse](https://github.com/kilfour/QuickPulse/blob/main/Docs/A_AQuickPulse/AQuickPulse.md) for a hands-on quickstart.  
## License

This project is licensed under the [MIT License](https://github.com/kilfour/QuickPulse/blob/main//LICENSE).  
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

[The origin story](./why-quickpulse-exists.md)  
