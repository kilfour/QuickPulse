namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Updates the stored value when the flag is true. Use for simple conditional state changes.
    /// </summary>
    public static Flow<TCell> ManipulateIf<TCell>(bool flag, Func<TCell, TCell> manipulate) =>
        Transduce(Flag(flag), ManipulatedValue(manipulate), SetTheCell<TCell>());

    /// <summary>
    /// Updates the stored value when the predicate evaluates to true for the current state. Use for context-sensitive mutations.
    /// </summary>
    public static Flow<TCell> ManipulateIf<TCell>(Func<TCell, bool> predicate, Func<TCell, TCell> manipulate) =>
        Transduce(Gate(predicate), ManipulatedValue(manipulate), SetTheCell<TCell>());
}
