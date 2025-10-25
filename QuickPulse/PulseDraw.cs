using QuickPulse.Bolts;

namespace QuickPulse;

public static partial class Pulse
{
    /// <summary>
    /// Reads the current value of type T from memory without changing it. Use to recall state stored by Prime or Manipulate.
    /// </summary>
    public static Flow<T> Draw<T>() => s => Cask.Some(s, s.GetTheBox<T>().Value);

    /// <summary>
    /// Reads a value derived from a stored box type without altering state. 
    /// Use to extract a specific field or projection from previously primed or manipulated memory.
    /// </summary>
    public static Flow<T> Draw<TBox, T>(Func<TBox, T> func) => s => Cask.Some(s, func(s.GetTheBox<TBox>().Value));
}
