# Capillaries and Arterioles
> A.k.a. Pulse Regulation. Branching and conditional control in QuickPulse.
 

So far we've mostly seen flows that travel forever on.
Useful for things like declarative composition,
but where would we be without the ability to branch off an Artery into an Arteriole or even a Capillary.

QuickPulse provides the following ways to control the *direction and branching* of a flow.  
## Using a Ternary Conditional Operator (*If/Then/Else*)
```csharp
static Flow<Flow> flow(int input) =>
    input % 2 == 0
        ? Pulse.Trace("even")
        : Pulse.Trace("uneven");
// Pulse [1, 2, 3, 4, 5] => results in ["uneven", "even", "uneven", "even", "uneven"].
```
Prefer `Pulse.NoOp()` when you want an if/then without an else-branch:  
```csharp
static Flow<Flow> flow(int input) =>
    input % 2 == 0
        ? Pulse.Trace("even")
        : Pulse.NoOp();
// Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
```
*Note:* While the ternary operator works, QuickPulse provides more idiomatic ways to deal with conditional statemens, which we will look at below.  
## When
`Pulse.When` is the declarative equivalent of the ternary operator combined with `.NoOp()`.  
```csharp
static Flow<Flow> flow(int input) =>
    Pulse.When(input % 2 == 0, Pulse.Trace("even"));
// Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
```
## The `Pulse.{SomeMethod}If()` Variants
In a similar vein to the state aware utility overloads,
most `Pulse` methods have an `If` variant that allows for conditional execution.  

  
**Examples:**  
  
*Conditional tracing:*  
```csharp
static Flow<Flow> flow(int input) =>
    Pulse.TraceIf(input % 2 == 0, () => "even");
// Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
```
*Branching a flow:*  
```csharp
static Flow<Flow> even(int _) => Pulse.Trace("even");
static Flow<Flow> three(int _) => Pulse.Trace("three");
static Flow<Flow> flow(int input) =>
    from _ in Pulse.ToFlowIf(input % 2 == 0, even, () => input)
    from __ in Pulse.ToFlowIf(input == 3, three, () => input)
    select Flow.Continue;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "three", "even"].
```
*Counting even numbers using `ManipulateIf()`:*  
```csharp
static Flow<Flow> flow(int input) =>
    from _ in Pulse.Prime(() => 0)
    from __ in Pulse.ManipulateIf<int>(input % 2 == 0, a => a + 1)
    from ___ in Pulse.Draw<int>().Trace(a => $"{input}: {a}")
    select Flow.Continue;
// Pulse [1, 2, 3, 4, 5] => results in ["1: 0", "2: 1", "3: 1", "4: 2", "5: 2"].
```
## FirstOf
Pulse.FirstOf(...) lets you chain multiple conditional flows and automatically
runs the first one whose condition evaluates to true.
It's like a compact, declarative if / else if / else ladder for flows.  
```csharp
static Flow<Flow> flow(int input) =>
    from _ in Pulse.FirstOf(
        (() => input % 2 == 0, () => Pulse.Trace("even")),
        (() => input == 3, () => Pulse.Trace("three")))
    select Flow.Continue;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "three", "even"].
```
