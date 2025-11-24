# Pulse On Type
**`Pulse.OnType(...)`** Executes a subflow over a value or collection, conditionally.  
```csharp
var dogFlow =
    from dog in Pulse.Start<Dog>()
    from _ in Pulse.Trace(dog.Name)
    select dog;
var flow =
    from animal in Pulse.Start<Animal>()
    from _ in Pulse.OnType(dogFlow, () => animal)
    select animal;
```
A factory method can also be used.  
```csharp
var dogFlow =
    from dog in Pulse.Start<Dog>()
    from _ in Pulse.Trace(dog.Name)
    select dog;
var flow =
    from animal in Pulse.Start<Animal>()
    from _ in Pulse.OnType((Dog a) => Pulse.Trace(a.Name), () => animal)
    select animal;
```
Typing the lamda expression (`(Dog a)`) avoids the need for the type variables.  
