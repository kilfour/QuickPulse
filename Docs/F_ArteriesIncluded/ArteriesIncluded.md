# Arteries Included
QuickPulse comes with a couple of built-in arteries:  
## The Shunt, a.k.a. `/dev/null`
The **Shunt** is the default artery installed in every new signal.  
It implements the Null Object pattern: an inert artery that silently absorbs all data.
Any call to `Absorb()` on a shunt simply vanishes, no storage, no side effects, no errors.
This ensures that flows without an explicitly attached artery still execute safely.  
## The Collector
The **Collector** is a simple artery that **gathers** every absorbed value into an internal collection.
It is primarily used in tests and diagnostics to verify what data a signal emits.
Each call to `Absorb()` appends the incoming objects to the exhibit list, preserving order.
Think of it as a **curator** for your flows, nothing escapes notice, everything is archived for later inspection.

Example:  
```csharp
var collector = TheCollector.Exhibits<string>();
Signal.From<string>(a => Pulse.Trace(a))
    .SetArtery(collector)
    .Pulse("hello")
    .Pulse("collector");
// collector.TheExhibit now equals ["hello", "collector"]
```
## The Latch
The **Latch** is a tiny, type-safe last-value latch. It simply remembers the most recent value absorbed and exposes it via `Q`.  
This is ideal for tests and probes where you only care about what came out last.

Example:  
```csharp
var latch = TheLatch.Holds<string>();
Signal.From<string>(a => Pulse.Trace(a))
    .SetArtery(latch)
    .Pulse("hello")
    .Pulse("latch");
// latch.Q now equals "latch"
```
## The Ledger
The **Ledger**` is a **persistent artery**, it records every absorbed value into a file.
Where `TheCollector` keeps its exhibits in memory, `TheLedger` writes them down for posterity.
It is ideal for tracing long-running flows or auditing emitted data across multiple runs.
Think of it as your **flow accountant**, keeping a faithful record of every transaction.  

Example:
  
```csharp
var ledger = TheLedger.Records();
Signal.From<string>(a => Pulse.Trace(a))
    .SetArtery(ledger)
    .Pulse("hello")
    .Pulse("filesystem");
// File.ReadAllLines(...) now equals ["hello", "filesystem"]
```
When a filename is not explicitly provided, a unique file is automatically created in a .quickpulse directory
located at the solution root (i.e., the nearest parent directory containing a .sln file).  

The filename follows this pattern:
```bash
/solution/.quickpulse/quick-pulse-{unique-suffix}.log
```
This ensures that each run generates a distinct, traceable log file without overwriting previous logs.
  
You can, of course, pass in a custom filename.  
In that case, a `myfilename.log` file is created, still in the nearest parent directory that contains a `.sln` file.  

Example:  
```csharp
 TheLedger.Records("myfilename.log").FilePath;
```
Note that the `Ledger` will throw an exception if no `.sln` file can be found.  
The `TheLedger.Rewrites()` factory method does exactly what it says: it clears the file before logging.
This is an idiomatic way to log repeatedly to a file that should start out empty:  
## The String Catcher
This catcher quietly captures everything that flows through it, and returns it as a single string.  
It is especially useful in testing and example scenarios where the full trace output is needed as a value.

Use the static helper `TheString.Catcher()` to create a new catcher.  
You can get a hold of the string through the `.Whispers()` method.  
```csharp
var holden = TheString.Catcher();
Signal.From(
    from x in Pulse.Start<int>()
    from _ in Pulse.Trace("x = ")
    from __ in Pulse.Trace(42)
    select x)
.SetArtery(holden)
.Pulse(42);
var result = holden.Whispers(); // <=
// result now equals "x = 42"
```
You can also reset/clear the *caught* values using the `.Forgets()` method.  
