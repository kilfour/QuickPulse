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
Flow<Flow> flow(int input) =>
    from _1 in dot.Then(dot).Then(dot).Then(space) // <=
    from _2 in Pulse.Trace(input)
    select Flow.Continue;
// Pulse 42 => results in '... 42'.
```
## ToFlow
If `Then` is about sequence, `ToFlow` is about delegation. It executes another flow *as part* of the current one.  
```csharp
Flow<Flow> subflow(int input) =>
    Pulse.Draw<int>().Trace(a => input + a);
Flow<Flow> flow(int input) =>
    from _ in Pulse.Prime(() => 1)
    from __ in Pulse.ToFlow(subflow, input)    // <=
    select Flow.Continue;
// Pulse 41 => results in 42.
```
This lets you reuse a named or shared flow inside another.
The subflow inherits the same signal state, so memory cells and arteries are visible across layers.  
`ToFlow` can also iterate through collections:  
```csharp
Flow<Flow> subflow(int input) =>
    Pulse.Manipulate<int>(a => a + input).Dissipate();
Flow<Flow> flow(List<int> input) =>
    from _1 in Pulse.Prime(() => 0)
    from _2 in Pulse.ToFlow(subflow, input)
    from _3 in Pulse.Draw<int>().Trace(a => $"Sum = {a}")
    select Flow.Continue;
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
a =>
    dot.Then(dot).Then(dot).Then(space).Then(Pulse.Trace(a));
```
```csharp
a =>
    Pulse.Prime(() => 1)
        .Then(Pulse.ToFlow(b => Pulse.Draw<int>().Trace(c => b + c), a));
```
```csharp
numbers =>
    Pulse.Prime(() => 0)
        .Then(Pulse.ToFlow(a => Pulse.Manipulate<int>(b => a + b).Dissipate(), numbers))
        .Then(Pulse.Draw<int>().Trace(a => $"Sum = {a}"));
```
Ultimately, the choice between query syntax and method syntax comes down to readability and personal preference.
Query syntax often provides a more declarative, linear flow that clearly expresses the sequence of operations,
while method syntax can offer a more functional, compositional style that some developers find more natural.  

**Note:** The .`Dissipate()` extension method runs the targeted flow and discards its output, returning a `Flow<Flow>`.  
It's often used in method syntax to glue flows together seamlessly.  
