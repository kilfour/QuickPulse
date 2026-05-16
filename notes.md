# Pulse On Type
**`Pulse.OnType(...)`** Executes a subflow over a value or collection, conditionally.  
```csharp
static Flow<Flow> dogFlow(Dog dog) =>
    Pulse.Trace(dog.Name);
static Flow<Flow> flow(Animal animal) =>
    Pulse.OnType((Dog a) => dogFlow(a), () => animal);
```
A factory method can also be used.  
```csharp
static Flow<Flow> flow(Animal animal) =>
    Pulse.OnType((Dog a) => Pulse.Trace(a.Name), () => animal);
```
Typing the lamda expression (`(Dog a)`) avoids the need for the type variables.  
