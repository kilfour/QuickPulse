### 0.4.0: Speaking Fluently From the Heart

* Added `Flow<Flow>` extension methods for most `Pulse` static methods, allowing for fluent continuation chains.
* Removed the *state aware* `Trace` and `TraceIf` methods.
* Added a `Flow<T>` `Trace(...)` and `TraceIf(...)` extension method allowing values currently carried by a flow to be traced directly.
* `Trace` no longer implicitly reads signal state; state access is now explicit through `Draw<T>()`.
* Fluent chaining significantly reduces discarded range variables (`_`, `_1`, `_2`, ...), improving readability in method and mixed syntax flows.