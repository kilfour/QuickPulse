# Times Extensions

> A small exploration showing how to express repetition and early termination using QuickPulse flows.

## Overview

These extension methods demonstrate how standard looping constructs (`for`, `while`) can be reimagined as declarative flow compositions in QuickPulse.
They are built using `Signal.From`, `Pulse.Trace`, and `Pulse.StopFlowingIf`, combining tracing with stateful early termination.

### Why it matters

This example highlights QuickPulse's ability to model control structures as composable flows, not syntax. Even something as simple as a repeat loop becomes observable, testable, and pluggable into the artery system.

---

## API

### `Times(this int times, Action action)`

Runs the given action a fixed number of times.

```csharp
5.Times(() => Console.WriteLine("Hello"));
// Output:
// Hello
// Hello
// Hello
// Hello
// Hello
```

Internally, this builds a signal from a unit flow that traces the given action, and pulses it `times` times:

```csharp
Signal.From<Unit>(a => Pulse.Trace(Chain.It(action, Unit.Instance)))
    .Pulse(Enumerable.Repeat(Unit.Instance, times));
```

### `Times<T>(this int times, Func<T> func)`

Runs the function `times` times, returning an eager sequence of its results.

```csharp
var seq = 3.Times(() => Random.Shared.Next());
Assert.Equal(3, seq.Count());
```

Each call is traced through an artery, collected, and returned as an enumerable.

### `TimesUntil<T>(this int times, Predicate<T> predicate, Func<T> func)`

Runs the given function until either the maximum number of times is reached or the predicate returns `true`.
The terminating element (where predicate becomes `true`) is not included in the result.

```csharp
int n = 0;
var seq = 10.TimesUntil(x => x > 3, () => n++);
// => [0, 1, 2, 3]
```

Flow equivalent:

```csharp
Signal.From<Unit>(a =>
        from i in Pulse.Start<Unit>()
        let value = func()
        let stop = predicate(value)
        from _1 in Pulse.TraceIf(!stop, () => value)
        from _2 in Pulse.StopFlowingIf(stop)
        select i)
    .GetResult<T>(times);
```

---

## Concepts Illustrated

* **Declarative repetition:** Loops expressed as flows.
* **Side-effect tracing:** Each iteration observed through arteries.
* **Early termination:** Implemented using `Pulse.StopFlowingIf`.
* **Eager evaluation:** Sequence materialized immediately, consistent with QuickPulse semantics.

---

## Potential Extensions

* `TimesWhile` — continue while predicate remains `true`.
* `RepeatUntilSuccess` — rerun an action until a specific condition succeeds.
* `TraceEvery` — attach a trace artery to every iteration.

These could be used as real examples or demos in future QuickPulse documentation to show composable control flow.
