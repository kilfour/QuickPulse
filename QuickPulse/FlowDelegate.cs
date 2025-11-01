using QuickPulse.Bolts;

namespace QuickPulse;

/// <summary>
/// A single step in a QuickPulse pipeline: given the current State, 
/// produces the next value and state (as a Beat&lt;TValue&gt;), composable via LINQ.
/// </summary>
public delegate IBeat<TValue> Flow<out TValue>(State state);