# Circulation
> Make it flow, number one.  

While it is entirely possible, and sometimes weirdly intellectually satisfying,
to write an entire QuickPulse Flow as one big LINQ expression,
it would be silly to ignore one of the main strengths of the LINQy approach: Composability.

QuickPulse provides two main ways to achieve this.  
## Then
The `Then` combinator joins two flows sequentially while sharing the same internal state.
It's the flow-level equivalent of saying *do this, then that*.  
```csharp
var dot = Pulse.Trace(".");
var space = Pulse.Trace(" ");
var flow =
    from input in Pulse.Start<int>()
    from _1 in dot.Then(dot).Then(dot).Then(space) // <=
    from _2 in Pulse.Trace(input)
    select input;
// Pulse 42 => results in '... 42'.
```
## ToFlow
If `Then` is about sequence, `ToFlow` is about delegation. It executes another flow *as part* of the current one.  
```csharp
var subflow =
    from input in Pulse.Start<int>()
    from _ in Pulse.Trace<int>(a => input + a)
    select input;
var flow =
    from input in Pulse.Start<int>()
    from _ in Pulse.Prime(() => 1)
    from __ in Pulse.ToFlow(subflow, input)    // <=
    select input;
// Pulse 41 => results in 42.
```
This lets you reuse a named or shared flow inside another.
The subflow inherits the same signal state, so memory cells and arteries are visible across layers.  
`ToFlow` can also iterate through collections:  
```csharp
var subflow =
    from input in Pulse.Start<int>()
    from result in Pulse.Manipulate<int>(a => a + input)
    select input;
var flow =
    from input in Pulse.Start<List<int>>()
    from _1 in Pulse.Prime(() => 0)
    from _2 in Pulse.ToFlow(subflow, input)
    from _3 in Pulse.Trace<int>(a => $"Sum = {a}")
    select input;
// Pulse [1, 2, 3] => results in "Sum = 6".
```
This version of `ToFlow` is the declarative way to write what would otherwise be a `loop`, `foreach`, `for`, etcetera.  
## Query Syntax vs Method Syntax
> Maybe now is the time to talk about Kevin.  

Another feature of LINQ is the two syntactically different but computationally equal styles of expression.  
In general the query syntax is more declarative (*what* you want to do),
while the method syntax can be more practical (*how* it actually executes).  

QuickPulse offers two similar dialects. The examples above are written in what could be called QuickPulse **query syntax**.  
Here are the same examples rewritten using **method syntax**:  
```csharp
var dot = Pulse.Trace(".");
var space = Pulse.Trace(" ");
Pulse.Start<int>(a =>
    dot.Then(dot).Then(dot).Then(space).Then(Pulse.Trace(a)));
```
```csharp
Pulse.Start<int>(a =>
    Pulse.Prime(() => 1)
        .Then(Pulse.ToFlow(b => Pulse.Trace<int>(c => b + c), a)));
```
```csharp
Pulse.Start<List<int>>(numbers =>
    Pulse.Prime(() => 0)
        .Then(Pulse.ToFlow(a => Pulse.Manipulate<int>(b => a + b).Dissipate(), numbers))
        .Then(Pulse.Trace<int>(a => $"Sum = {a}")));
```
Ultimately, the choice between query syntax and method syntax comes down to readability and personal preference.
Query syntax often provides a more declarative, linear flow that clearly expresses the sequence of operations,
while method syntax can offer a more functional, compositional style that some developers find more natural.  

**Note:** The .`Dissipate()` extension method runs the targeted flow and discards its output, returning a `Flow<Flow>`.  
It's often used in method syntax to glue flows together seamlessly.  
