using QuickPulse.Bolts;

namespace QuickPulse;

/// <summary>
/// A single step in a QuickPulse pipeline: given the current State, produces the next value and state (as a Cask<T>), composable via LINQ.
/// </summary>
public delegate Cask<T> Flow<T>(State state);