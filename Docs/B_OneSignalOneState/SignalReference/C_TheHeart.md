# The Heart
> Hunting for Flows.

  
The Heart is the typed Artery registry: it remembers where pulses can go, based on Artery type, and lets you target them deliberately.  
It is *not* an output by itself. 
  
## The Main Artery
There is *always* exactly one Main Artery. It is the default outflow for a signal.  

**`Signal.SetArtery(...)`** sets the **Main Artery**.  
All `Pulse.Trace(...)` and `Pulse.TraceIf(...)` emissions flow into it.    
```csharp
Signal.From<int>(a => Pulse.Trace(a))
    .SetArtery(holden) // <= 'holden' is now the Main Artery
    .Pulse(42);
```
**`Signal.SetAndReturnArtery(...)`** Similar, but returns the Artery you pass in (useful for quick wiring):  
```csharp
 Signal.From<int>(a => Pulse.Trace(a)).SetAndReturnArtery(TheString.Catcher());
```
Setting an Artery on a signal that already has one **replaces** the previous Artery.    
```csharp
var holden = TheString.Catcher();
var caulfield = TheString.Catcher();
Signal.From<int>(a => Pulse.Trace(a))
    .SetArtery(holden)
    .Pulse(42)
    .SetArtery(caulfield)
    .Pulse(43);
Assert.Equal("42", holden.Whispers());
Assert.Equal("43", caulfield.Whispers());
```
#### Safeties:
- Trying to set the Main Artery to null throws:  
    > The Heart can't pump into null. Did you pass a valid Artery to SetArtery(...) ?  
## Grafting Arteries
Apart from pulsing flows through the Main Artery, QuickPulse allows you to redirect flows to additional Arteries.  
There are various situations where this is useful.  
In the following section we will discuss how to set up one particular use case:
'Adding a diagnostic trace to an existing flow.' 

Suppose we have the following flow:   
```csharp
return
    from ch in Pulse.Start<char>()
    from depth in Pulse.Prime(() => -1)
    from _ in Pulse.TraceIf(depth >= 0, () => ch)
    from __ in Pulse.ManipulateIf<int>(ch == '{', x => x + 1)
    from ___ in Pulse.ManipulateIf<int>(ch == '}', x => x - 1)
    select ch;
```
This is a simple flow that returns the text between braces, even if there are other braces inside said text.  
**An Example**:  
```csharp
var holden = TheString.Catcher();
Signal.From(flow)
    .SetArtery(holden)
    .Pulse("{ a { b } c }");
```
Unfortunately the result of this is ` a { b } c }` and really, we want it to be ` a { b } c `.    

So let's try and find out what's going on.  

First we define a new typed Artery:  
```csharp
public class Diagnostic : Collector<string> { }
```
Then we *Graft* it onto the Heart through the `Signal.Graft(...)` method.  
```csharp
var holden = TheString.Catcher();
var diagnostic = new Diagnostic();
Signal.From(flow)
    .SetArtery(holden)
    .Graft(diagnostic)
    .Pulse("{ a { b } c }");
```
In this case, we could just Graft the `TheCollector<string>`, but creating a derived class explains our intent much better.

Lastly we add a `Pulse.TraceTo<TArtery>(...)` to the flow:
  
```csharp
var flow = 
    from ch in Pulse.Start<char>()
    from depth in Pulse.Prime(() => -1)
    let enter = depth
    let emit = depth >= 0
    from _ in Pulse.TraceIf(emit, () => ch)
    from __ in Pulse.ManipulateIf<int>(ch == '{', x => x + 1)
    from ___ in Pulse.ManipulateIf<int>(ch == '}', x => x - 1)
    from exit in Pulse.Draw<int>()
    from diag in Pulse.TraceTo<Diagnostic>(
        $"char='{ch}', enter={enter}, emit={emit}, exit={exit}")
    select ch;
```
When executing this, the `Holden` Artery contains the same as before, but now we have the following in the `Diagnostic` Artery:  
```csharp
[
    "char='{', enter=-1, emit=False, exit=0",
    "char=' ', enter=0, emit=True, exit=0",
    "char='a', enter=0, emit=True, exit=0",
    "char=' ', enter=0, emit=True, exit=0",
    "char='{', enter=0, emit=True, exit=1",
    "char=' ', enter=1, emit=True, exit=1",
    "char='b', enter=1, emit=True, exit=1",
    "char=' ', enter=1, emit=True, exit=1",
    "char='}', enter=1, emit=True, exit=0",
    "char=' ', enter=0, emit=True, exit=0",
    "char='c', enter=0, emit=True, exit=0",
    "char=' ', enter=0, emit=True, exit=0",
    "char='}', enter=0, emit=True, exit=-1"
];
```
We can now use this information to correct the original flow  
## GetArtery
**`Signal.GetArtery<TArtery>(...)`** can be used to retrieve the current `IArtery` set on the signal.  
  
```csharp
var holden =
    Signal.From<int>(a => Pulse.Trace(a))
        .SetArtery(TheString.Catcher())
        .Pulse(42)
        .GetArtery<Holden>();
Assert.Equal("42", holden.Whispers());
```
