# Capillaries and Arterioles
> A.k.a. Pulse Regulation. Branching and conditional control in QuickPulse.
 

So far we've mostly seen flows that travel forever on.
Useful for things like declarative composition,
but where would we be without the ability to branch off an Artery into an Arteriole or even a Capillary.

QuickPulse provides the following ways to control the *direction* of a flow.  
## Using a Ternary Conditional Operator (*If/Then/Else*)
```csharp
var flow =
    from input in Pulse.Start<int>()
    let conditional =
        input % 2 == 0
        ? Pulse.Trace("even")
        : Pulse.Trace("uneven")
    from _ in conditional
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["uneven", "even", "uneven", "even", "uneven"].
```
Prefer `Pulse.NoOp()` when you want an if/then without an else-branch:  
```csharp
var flow =
    from input in Pulse.Start<int>()
    let conditional =
        input % 2 == 0
        ? Pulse.Trace("even")
        : Pulse.NoOp()
    from _ in conditional
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
```
*Note:* While the ternary operator works, QuickPulse provides more idiomatic ways to deal with conditional statemens, which we will look at below.  
## When
`Pulse.When` is the declarative equivalent of the ternary operator combined with `.NoOp()`.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.When(input % 2 == 0, Pulse.Trace("even"))
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
```
## The `Pulse.{SomeMethod}If()` Variants
In a similar vein to the state aware utility overloads,
most `Pulse` methods have an `If` variant that allows for conditional execution.  

  
**Examples:**  
  
*Conditional tracing:*  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.TraceIf(input % 2 == 0, () => "even")
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "even"].
```
*Branching a flow:*  
```csharp
var even = Pulse.Start<int>(_ => Pulse.Trace("even"));
var three = Pulse.Start<int>(_ => Pulse.Trace("three"));
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.ToFlowIf(input % 2 == 0, even, () => input)
    from __ in Pulse.ToFlowIf(input == 3, three, () => input)
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "three", "even"].
```
*Counting even numbers using `ManipulateIf()`:*  
```csharp
var even = Pulse.Start<int>(_ => Pulse.Trace("even"));
var three = Pulse.Start<int>(_ => Pulse.Trace("three"));
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.Prime(() => 0)
    from __ in Pulse.ManipulateIf<int>(input % 2 == 0, a => a + 1)
    from ___ in Pulse.Trace<int>(a => $"{input}: {a}")
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["1: 0", "2: 1", "3: 1", "4: 2", "5: 2"].
```
## FirstOf
Pulse.FirstOf(...) lets you chain multiple conditional flows and automatically
runs the first one whose condition evaluates to true.
It's like a compact, declarative if / else if / else ladder for flows.  
```csharp
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.FirstOf(
        (() => input % 2 == 0, () => Pulse.Trace("even")),
        (() => input == 3, () => Pulse.Trace("three")))
    select input;
// Pulse [1, 2, 3, 4, 5] => results in ["even", "three", "even"].
```
