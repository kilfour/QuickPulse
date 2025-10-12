using QuickPulse.Arteries;
using QuickPulse.Explains;

namespace QuickPulse.Tests;

[DocFile]
[DocFileHeader("<img src='icon.png' width='40' align='top'/> QuickPulse")]
[DocContent(
@"
> **LINQ with a heartbeat.**

A tiny library for building stateful, inspectable, composable flows.

[![Docs](https://img.shields.io/badge/docs-QuickPulse-blue?style=flat-square&logo=readthedocs)](https://github.com/kilfour/QuickPulse/blob/main/Docs/ToC.md)
[![NuGet](https://img.shields.io/nuget/v/QuickPulse.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/QuickPulse)
[![License: MIT](https://img.shields.io/badge/license-MIT-success?style=flat-square)](https://github.com/kilfour/QuickPulse/blob/main/LICENSE)")]
public class ReadMe
{
    [Fact]
    [DocHeader("Getting Started")]
    [DocContent("QuickPulse lets you write LINQ expressions that react to values, accumulate state, and emit traces along the way.")]
    [DocExample(typeof(ReadMe), nameof(SignalExample))]
    public void Example() =>
        Assert.Equal("A deep dark forest, a looking glass and a trail of dead generators.", SignalExample());

    [CodeSnippet]
    [CodeRemove("return")]
    private static string SignalExample()
    {
        return Signal.From(
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
    }
    [DocHeader("Highlights")]
    [DocContent(@"
* Composable `Flow<T>` pipelines using LINQ.
* Stateful `Signal<T>` execution.
* Built-in tracing via `IArtery`.
* Conditional logic.
* Scoped state and reversible manipulations.
* Integration with file logging and value capture.")]
    private static void Highlights() { }

    [DocHeader("Installation")]
    [DocContent(@"
QuickPulse is available on NuGet:
```bash
Install-Package QuickPulse
```
Or via the .NET CLI:
```bash
dotnet add package QuickPulse
```")]
    private static void Installation() { }

    [Fact(Skip = "explicit")]
    [DocHeader("Documentation")]
    [DocContent(@"
QuickPulse is fully documented, with real, executable examples for each combinator and concept.

You can explore it here:

* **[Table of Contents](https://github.com/kilfour/QuickPulse/blob/main/Docs//ToC.md)**

Or, ... see [A Quick Pulse](https://github.com/kilfour/QuickPulse/blob/main/Docs/A_AQuickPulse/AQuickPulse.md) for a hands-on quickstart.")]
    public void Documentation() { Explain.OnlyThis<ReadMe>("README.md"); }

    [DocHeader("License")]
    [DocContent(@"
This project is licensed under the [MIT License](https://github.com/kilfour/QuickPulse/blob/main//LICENSE).")]
    private static void License() { }

    [DocHeader("Why QuickPulse Exists?")]
    [DocContent(@"
Do you know how you sometimes leave your house, maybe to get some cigarettes, and start thinking about something?
Your brain takes over.
You walk straight past the shop, and the legs just keep going.
An hour later, you look up, and you're in the next village wondering how you got there.

No? Just me?

Well, okay.

It happens in code too, ... quite a lot.
This library is the result of one of those walks through a dark forest.
And yes, it did *literally* involve Trees.")]
    private static void WhyQuickPulseExists() { }

}