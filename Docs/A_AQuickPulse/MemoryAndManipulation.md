# Memory And Manipulation
> How QuickPulse remembers, updates, and temporarily alters state.  
## Prime: one-time lazy initialization.
`Prime(() => T)` computes and stores a value **once per signal lifetime**.  
## Draw: read from memory.
`Draw<T>()` retrieves the current value from the signal's memory for type `T`.  
## Manipulate: controlled mutation of *primed* state.
`Manipulate<T>(Func<T,T>)` updates the current value of the gathered cell for type `T`.  
## 
  
