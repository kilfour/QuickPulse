using QuickPulse.Bolts;

namespace QuickPulse;

/// <summary>
/// A single step in a QuickPulse pipeline: given the current State, 
/// produces the next value and state (as a Beat&lt;TValue&gt;), composable via LINQ.
/// </summary>
public delegate Beat<TValue> Flow<TValue>(State state);