# Memory And Manipulation
> How QuickPulse remembers, updates, and temporarily alters state.  
## Prime: one-time lazy initialization.
`Prime(() => T)` computes and stores a value **once per signal lifetime**.  
## Draw: read from memory.
`Draw<T>()` retrieves the current value from the signal's memory for type `T`.  
## Manipulate: controlled mutation of *primed* state.
`Manipulate<T>(Func<T,T>)` updates the current value of the gathered cell for type `T`.  
## Scoped: temporary overrides with automatic restore.
`Scoped<T>(enter, innerFlow)` runs `innerFlow` with a **temporary** value for the gathered cell of type `T`. On exit, the outer value is restored.  
Any `Manipulate<T>` inside the scope affects the **scoped** value and is discarded on exit.  
Any `Manipulate<T>` inside the scope affects the **scoped** value and is discarded on exit.  
* **Type identity matters**: Use wrapper records to keep multiple cells of the same underlying type.  
