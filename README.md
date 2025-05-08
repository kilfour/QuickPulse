## QuickPulse
Do you know how you sometimes leave your house, to get some cigarettes for instance.  
But you start thinking about something and your brain just takes over. 
So you walk straight past the shop and the legs keep going.  
An hour later you look up, and you're in the next village wondering how you got there.  

No ? ... Just me ?  
Well, ok.  

It happens in code too though, ... quite a lot.  
This library is the result of one of those walks through a dark forest, 
and yes it did *literally* involved Trees.


---
## Building a Flow
In order to explain how QuickPulse works, not in the least to myself, let's walk through 
building up a flow, for a real world use case.


The minimal definition of a flow :
```csharp
from _ in Pulse.Start<DocAttribute>()
select Pulse.Stop;
```
The type generic in `Pulse.Start` defines the **input type** to the flow.  
This means you'll call it like this:
```csharp
flow.Run(new DocAttribute());
```


---
`Pulse.Stop` is just semantic sugar, a readable way to express that the return value is irrelevant.  
While you could select anything (including real return values),
using Pulse.Stop makes your intent clear to both the compiler and future readers.

---
**Adding a Trace:**
You might have guessed were building a document generator thingy, 



---
**Adding a Shape:**




