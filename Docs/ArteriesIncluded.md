# Arteries Included
QuickPulse comes with three built-in arteries:

## TheCollector
This is the artery used throughout the documentation examples, and it's especially useful in testing scenarios.

Example:
```csharp
var collector = new TheCollector<string>();
Signal.Tracing<string>()
    .SetArtery(collector)
    .Pulse("hello", "collector");
Assert.Equal("hello", collector.TheExhibit[0]);
Assert.Equal("collector", collector.TheExhibit[1]);
```


## WriteDataToFile
This artery is included because writing trace output to a file is one of the most common use cases.
Example:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile())
    .Pulse("hello", "collector");
```
The file will contain:
```
hello
collector
```


When a filename is not explicitly provided, a unique file is automatically created in a .quickpulse directory
located at the solution root (i.e., the nearest parent directory containing a .sln file).  

The filename follows this pattern:
```bash
/solution/.quickpulse/quick-pulse-{unique-suffix}.log
```
This ensures that each run generates a distinct, traceable log file without overwriting previous logs.


You can, of course, pass in a custom filename.
Example:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile("myfilename.log"))
    .Pulse("hello", "collector");
```
In that case, a `myfilename.log` file is created, still in the nearest parent directory that contains a `.sln` file.


Note that the `WriteDataToFile` constructor will throw an exception if no `.sln` file can be found.

To avoid solution root detection altogether, use the following factory method:
```csharp
Signal.Tracing<string>()
    .SetArtery(WriteDataToFile.UsingHardCodedPath("hard.txt"))
    .Pulse("hello", "collector");
```


`WriteDataToFile` appends all entries to the file; each pulse adds new lines to the end.


The `ClearFile` method does exactly what it says: it clears the file before logging.
This is an idiomatic way to log repeatedly to a file that should start out empty:
```csharp
Signal.Tracing<string>()
    .SetArtery(new WriteDataToFile().ClearFile())
    .Pulse("hello", "collector");
```


### Sugaring
These are simple aliases that make common cases easier to read:
- `WriteData.ToFile(...)` is shorthand for `new WriteDataToFile(...)`

- `WriteData.ToNewFile(...)` is shorthand for `new WriteDataToFile(...).ClearFile()`

## TheStringCatcher
This catcher quietly captures everything that flows through it, and returns it as a single string.  
It is especially useful in testing and example scenarios where the full trace output is needed as a value.

Use the static helper `TheString.Catcher()` to create a new catcher:
```csharp
var holden = TheString.Catcher();
```

You can get a hold of the string through the `.Whispers()` method.
```csharp
var holden = TheString.Catcher();
Signal.From(
        from x in Pulse.Start<int>()
        from _ in Pulse.Trace($"x = {x}")
        select x)
    .SetArtery(holden)
    .Pulse(42);
Assert.Equal("x = 42", holden.Whispers());
```
Great for assertions or string comparisons in tests.

