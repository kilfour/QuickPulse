namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Updates the stored value of type TCell using the given function and emits the new value. Use to evolve state between pulses.
    /// </summary>
    public static Flow<TCell> Manipulate<TCell>(Func<TCell, TCell> manipulate) =>
        Transduce(Always, ManipulatedValue(manipulate), SetTheCell<TCell>());
}