namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Updates the stored value of type T using the given function and emits the new value. Use to evolve state between pulses.
    /// </summary>
    public static Flow<T> Manipulate<T>(Func<T, T> manipulate) =>
        Transduce(Always, ManipulatedValue(manipulate), SetTheCell<T>());
}