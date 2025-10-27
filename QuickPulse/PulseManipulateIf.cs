namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Updates the stored value when the flag is true. Use for simple conditional state changes.
    /// </summary>
    public static Flow<T> ManipulateIf<T>(bool flag, Func<T, T> manipulate) =>
        Transduce(Flag(flag), ManipulatedValue(manipulate), SetTheCell<T>());

    /// <summary>
    /// Updates the stored value when the predicate evaluates to true for the current state. Use for context-sensitive mutations.
    /// </summary>
    public static Flow<T> ManipulateIf<T>(Func<T, bool> predicate, Func<T, T> manipulate) =>
        Transduce(Gate(predicate), ManipulatedValue(manipulate), SetTheCell<T>());
}
